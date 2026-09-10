using Equiparts.Models.Auth;
using Equiparts.Models.Profile;
using Refit;

namespace Equiparts.Interfaces;

// Authenticated endpoints: registered WITH AuthHandler.
public interface IProfileApi
{
    [Get("/api/profile")]
    Task<ApiResult<ProfileResponse>> GetProfileAsync(CancellationToken cancellationToken = default);

    [Put("/api/profile")]
    Task<ApiResult<ProfileResponse>> UpdateProfileAsync([Body] ProfileUpdateRequest request, CancellationToken cancellationToken = default);

    [Post("/api/profile/change-password")]
    Task<ApiResult<object?>> ChangePasswordAsync([Body] ChangePasswordRequest request, CancellationToken cancellationToken = default);

    [Get("/api/profile/addresses")]
    Task<ApiResult<List<AddressResponse>>> GetAddressesAsync(CancellationToken cancellationToken = default);

    [Post("/api/profile/addresses")]
    Task<ApiResult<AddressResponse>> AddAddressAsync([Body] AddressRequest request, CancellationToken cancellationToken = default);

    [Put("/api/profile/addresses/{addressId}")]
    Task<ApiResult<AddressResponse>> UpdateAddressAsync(int addressId, [Body] AddressRequest request, CancellationToken cancellationToken = default);

    [Delete("/api/profile/addresses/{addressId}")]
    Task<ApiResult<object?>> DeleteAddressAsync(int addressId, CancellationToken cancellationToken = default);
}
