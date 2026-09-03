namespace Equiparts.Interfaces;

// Owns token storage (SecureStorage) and refresh coordination. AuthHandler calls into
// this to obtain/refresh a valid access token; it never talks to SecureStorage directly.
public interface ITokenService
{
    // Raised when the refresh token itself is missing, expired, or rejected by the
    // server - i.e. the session cannot be recovered and the user must log in again.
    event EventHandler? SessionExpired;

    Task<bool> HasStoredSessionAsync();

    // Returns a token that is valid per the locally-stored expiry, refreshing first if
    // needed. Refresh attempts are single-flight: concurrent callers share one refresh.
    // Returns null if there is no session to use.
    Task<string?> GetValidAccessTokenAsync(CancellationToken cancellationToken = default);

    // Called after the server rejects a request with 401 despite a locally-valid token.
    // failedAccessToken lets the single-flight lock detect a refresh that already
    // happened while this caller was waiting, so it can reuse it instead of refreshing again.
    Task<string?> ForceRefreshAccessTokenAsync(string? failedAccessToken, CancellationToken cancellationToken = default);

    Task SaveTokensAsync(string accessToken, string refreshToken, DateTime accessTokenExpiresUtc);

    Task ClearTokensAsync();
}
