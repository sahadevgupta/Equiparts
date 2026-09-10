using System.Net;
using Equiparts.Configuration.Mapper;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Models.Profile;
using Microsoft.Extensions.Logging;
using Refit;

namespace Equiparts.Services;

public class ProfileService(IProfileApi profileApi,
    IConnectivityService connectivityService,
    ILogger<ProfileService> logger) : IProfileService
{
    public async Task<UserProfile?> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);
            var response = await profileApi.GetProfileAsync(cancellationToken);

            if (response is not { Success: true, Data: not null })
                return null;

            return BackendToAppModelMapper.GetUserProfile(response.Data);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to fetch profile ({StatusCode}).", apiEx.StatusCode);
            return null;
        }
    }

    public async Task<UserProfile?> UpdateProfileAsync(string fullName, string email, string? phoneNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);

            var request = new ProfileUpdateRequest
            {
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber
            };

            var response = await profileApi.UpdateProfileAsync(request, cancellationToken);

            if (response is not { Success: true, Data: not null })
                return null;

            return BackendToAppModelMapper.GetUserProfile(response.Data);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to update profile ({StatusCode}).", apiEx.StatusCode);
            return null;
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);

            var request = new ChangePasswordRequest
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };

            var response = await profileApi.ChangePasswordAsync(request, cancellationToken);

            return response.Success
                ? (true, null)
                : (false, response.Message ?? "Failed to change password. Please try again.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to change password ({StatusCode}).", apiEx.StatusCode);
            var message = apiEx.StatusCode == HttpStatusCode.Unauthorized || apiEx.StatusCode == HttpStatusCode.BadRequest
                ? "Current password is incorrect."
                : "Failed to change password. Please try again.";
            return (false, message);
        }
    }

    public async Task<List<Address>> GetAddressesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);
            var response = await profileApi.GetAddressesAsync(cancellationToken);

            if (response is not { Success: true, Data: not null })
                return [];

            return BackendToAppModelMapper.GetAddresses(response.Data);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to fetch addresses ({StatusCode}).", apiEx.StatusCode);
            return [];
        }
    }

    public async Task<Address?> AddAddressAsync(Address address, CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);
            var response = await profileApi.AddAddressAsync(ToAddressRequest(address), cancellationToken);

            if (response is not { Success: true, Data: not null })
                return null;

            return BackendToAppModelMapper.GetAddress(response.Data);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to add address ({StatusCode}).", apiEx.StatusCode);
            return null;
        }
    }

    public async Task<Address?> UpdateAddressAsync(int addressId, Address address, CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);
            var response = await profileApi.UpdateAddressAsync(addressId, ToAddressRequest(address), cancellationToken);

            if (response is not { Success: true, Data: not null })
                return null;

            return BackendToAppModelMapper.GetAddress(response.Data);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to update address ({StatusCode}).", apiEx.StatusCode);
            return null;
        }
    }

    public async Task<bool> DeleteAddressAsync(int addressId, CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);
            var response = await profileApi.DeleteAddressAsync(addressId, cancellationToken);

            return response.Success;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to delete address ({StatusCode}).", apiEx.StatusCode);
            return false;
        }
    }

    private static AddressRequest ToAddressRequest(Address address)
    {
        return new AddressRequest
        {
            Label = address.Label,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country,
            PhoneNumber = address.PhoneNumber,
            IsDefault = address.IsDefault
        };
    }
}
