using Equiparts.Models.Auth;
using Refit;

namespace Equiparts.Interfaces;

// Example authenticated endpoint - registered WITH AuthHandler. Add real business
// API interfaces (orders, catalog, etc.) alongside this using the same registration pattern.
public interface IUserApi
{
    [Get("/api/users/profile")]
    Task<ApiResult<UserProfileResponse>> GetProfileAsync(CancellationToken cancellationToken = default);
}
