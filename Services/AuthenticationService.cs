using System.Net;
using Equiparts.Configuration.Mapper;
using Equiparts.Interfaces;
using Equiparts.Models.Auth;
using Microsoft.Extensions.Logging;
using Refit;

namespace Equiparts.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthApi _authApi;
    private readonly ITokenService _tokenService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IAuthApi authApi,
        ITokenService tokenService,
        ICurrentUserService currentUserService,
        ILogger<AuthenticationService> logger)
    {
        _authApi = authApi;
        _tokenService = tokenService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _authApi.LoginAsync(new LoginRequest { Email = email, Password = password }, cancellationToken);

            if (response is not { Success: true, Data: not null })
                return (false, response.Message ?? "Login failed. Please try again.");

            await _tokenService.SaveTokensAsync(response.Data.AccessToken, response.Data.RefreshToken, response.Data.AccessTokenExpiresUtc);

            var userSession = BackendToAppModelMapper.GetUserSession(response.Data);
            if (userSession is not null)
                _currentUserService.Set(userSession);

            _logger.LogInformation("Login successful.");
            return (true, null);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            _logger.LogWarning(apiEx, "Login rejected by server ({StatusCode}).", apiEx.StatusCode);
            var message = apiEx.StatusCode == HttpStatusCode.Unauthorized
                ? "Invalid email or password."
                : "Login failed. Please try again.";
            return (false, message);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or TimeoutException)
        {
            _logger.LogWarning(ex, "Login failed due to a network error.");
            return (false, "Unable to reach the server. Please check your connection.");
        }
    }

    public async Task LogoutAsync()
    {
        _logger.LogInformation("Logging out.");
        await _tokenService.ClearTokensAsync();
        _currentUserService.Clear();
    }
}
