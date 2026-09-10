using Equiparts.Models.Auth;
using Refit;

namespace Equiparts.Interfaces;

// Unauthenticated endpoints: registered WITHOUT AuthHandler so login/refresh never
// recurse back through the token-refresh pipeline.
public interface IAuthApi
{
    [Post("/api/auth/login")]
    Task<ApiResult<LoginResponse>> LoginAsync([Body] LoginRequest request, CancellationToken cancellationToken = default);

    // Returns the same shape as login - the backend logs the new user in immediately
    // after registration rather than requiring a separate login call.
    [Post("/api/auth/register")]
    Task<ApiResult<LoginResponse>> RegisterAsync([Body] RegisterRequest request, CancellationToken cancellationToken = default);

    [Post("/api/auth/refresh")]
    Task<ApiResult<LoginResponse>> RefreshTokenAsync([Body] RefreshTokenRequest request, CancellationToken cancellationToken = default);
}
