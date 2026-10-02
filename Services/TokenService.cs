using System.Globalization;
using System.Net;
using Equiparts.Configuration;
using Equiparts.Interfaces;
using Equiparts.Models.Auth;
using Microsoft.Extensions.Logging;
using Refit;

namespace Equiparts.Services;

public sealed class TokenService : ITokenService
{
    private const string AccessTokenKey = "equiparts_access_token";
    private const string RefreshTokenKey = "equiparts_refresh_token";
    private const string AccessTokenExpiryKey = "equiparts_access_token_expires_utc";

    private readonly IAuthApi _authApi;
    private readonly IConnectivityService _connectivityService;
    private readonly ILogger<TokenService> _logger;

    // Single-flight lock: every concurrent caller awaits the same refresh instead of
    // each triggering its own call to the refresh endpoint.
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public event EventHandler? SessionExpired;

    public TokenService(IAuthApi authApi, IConnectivityService connectivityService, ILogger<TokenService> logger)
    {
        _authApi = authApi;
        _connectivityService = connectivityService;
        _logger = logger;
    }

    public async Task<bool> HasStoredSessionAsync()
    {
        var refreshToken = await GetStoredRefreshTokenAsync();
        return !string.IsNullOrEmpty(refreshToken);
    }

    public async Task<string?> GetValidAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var accessToken = await GetStoredAccessTokenAsync();
        var refreshToken = await GetStoredRefreshTokenAsync();

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            return null;

        if (!IsExpired(await GetStoredExpiryAsync()))
            return accessToken;

        if (!_connectivityService.IsConnected)
        {
            _logger.LogWarning("Access token expired but device is offline; using cached token for this attempt.");
            return accessToken;
        }

        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            // Another request may have already refreshed while we waited for the lock.
            accessToken = await GetStoredAccessTokenAsync();
            if (!IsExpired(await GetStoredExpiryAsync()))
                return accessToken;

            _logger.LogInformation("Token expired. Refreshing access token.");
            return await PerformRefreshAsync(cancellationToken);
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<string?> ForceRefreshAccessTokenAsync(string? failedAccessToken, CancellationToken cancellationToken = default)
    {
        if (!_connectivityService.IsConnected)
        {
            _logger.LogWarning("API returned 401 but device is offline; skipping refresh attempt.");
            return null;
        }

        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            var currentAccessToken = await GetStoredAccessTokenAsync();

            // Someone else already refreshed while we were waiting for the lock - reuse
            // the new token instead of refreshing a second time.
            if (!string.IsNullOrEmpty(currentAccessToken) &&
                !string.Equals(currentAccessToken, failedAccessToken, StringComparison.Ordinal))
            {
                return currentAccessToken;
            }

            _logger.LogInformation("API returned 401. Refreshing access token.");
            return await PerformRefreshAsync(cancellationToken);
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    // Callers must hold _refreshLock before calling this.
    private async Task<string?> PerformRefreshAsync(CancellationToken cancellationToken)
    {
        var refreshToken = await GetStoredRefreshTokenAsync();
        var accessToken = await GetStoredAccessTokenAsync();

        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger.LogWarning("No refresh token available. Session expired.");
            await ClearTokensAsync();
            SessionExpired?.Invoke(this, EventArgs.Empty);
            return null;
        }

        try
        {
            var response = await _authApi.RefreshTokenAsync(new RefreshTokenRequest { AccessToken = accessToken, RefreshToken = refreshToken }, cancellationToken);

            if (response is { Success: true, Data: not null })
            {
                // Always store both values returned - the backend may rotate the refresh token.
                await SaveTokensAsync(response.Data.AccessToken, response.Data.RefreshToken, response.Data.AccessTokenExpiresUtc);
                _logger.LogInformation("Token refresh successful.");
                return response.Data.AccessToken;
            }

            _logger.LogWarning("Token refresh rejected by server. Session expired.");
            await ClearTokensAsync();
            SessionExpired?.Invoke(this, EventArgs.Empty);
            return null;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx) when (apiEx.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            _logger.LogWarning("Refresh token rejected ({StatusCode}). Session expired.", apiEx.StatusCode);
            await ClearTokensAsync();
            SessionExpired?.Invoke(this, EventArgs.Empty);
            return null;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or TimeoutException)
        {
            // Transient/network failure - keep the session intact so the next attempt can retry.
            _logger.LogWarning(ex, "Token refresh failed due to a network error.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while refreshing access token.");
            return null;
        }
    }

    public async Task SaveTokensAsync(string accessToken, string refreshToken, DateTime accessTokenExpiresUtc)
    {
        // A timestamp sent with an offset (e.g. +05:30) is deserialized as Local and must be
        // converted, not relabelled - SpecifyKind alone would shift expiry by the device's
        // UTC offset. Offset-less values are taken as UTC, per the field's contract.
        var expiresUtc = accessTokenExpiresUtc.Kind switch
        {
            DateTimeKind.Utc => accessTokenExpiresUtc,
            DateTimeKind.Local => accessTokenExpiresUtc.ToUniversalTime(),
            _ => DateTime.SpecifyKind(accessTokenExpiresUtc, DateTimeKind.Utc)
        };

        await SecureStorage.Default.SetAsync(AccessTokenKey, accessToken);
        await SecureStorage.Default.SetAsync(RefreshTokenKey, refreshToken);
        await SecureStorage.Default.SetAsync(AccessTokenExpiryKey, expiresUtc.ToString("O", CultureInfo.InvariantCulture));
    }

    public Task ClearTokensAsync()
    {
        SecureStorage.Default.Remove(AccessTokenKey);
        SecureStorage.Default.Remove(RefreshTokenKey);
        SecureStorage.Default.Remove(AccessTokenExpiryKey);
        return Task.CompletedTask;
    }

    private static Task<string?> GetStoredAccessTokenAsync() => SecureStorage.Default.GetAsync(AccessTokenKey);

    private static Task<string?> GetStoredRefreshTokenAsync() => SecureStorage.Default.GetAsync(RefreshTokenKey);

    private static async Task<DateTime?> GetStoredExpiryAsync()
    {
        var raw = await SecureStorage.Default.GetAsync(AccessTokenExpiryKey);

        if (string.IsNullOrEmpty(raw))
            return null;

        return DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
            ? parsed
            : null;
    }

    private static bool IsExpired(DateTime? expiresUtc)
    {
        // Malformed/missing expiry is treated as expired so a refresh is forced rather
        // than trusting an access token we can't validate the lifetime of.
        if (expiresUtc is null)
            return true;

        return DateTime.UtcNow >= expiresUtc.Value.Subtract(ApiConstants.AccessTokenExpiryBuffer);
    }
}
