using Equiparts.Models;

namespace Equiparts.Interfaces;

public interface IProfileService
{
    Task<UserProfile?> GetProfileAsync(CancellationToken cancellationToken = default);

    Task<UserProfile?> UpdateProfileAsync(string fullName, string email, string? phoneNumber, CancellationToken cancellationToken = default);

    Task<(bool Success, string? ErrorMessage)> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    Task<List<Address>> GetAddressesAsync(CancellationToken cancellationToken = default);

    Task<Address?> AddAddressAsync(Address address, CancellationToken cancellationToken = default);

    Task<Address?> UpdateAddressAsync(int addressId, Address address, CancellationToken cancellationToken = default);

    Task<bool> DeleteAddressAsync(int addressId, CancellationToken cancellationToken = default);
}
