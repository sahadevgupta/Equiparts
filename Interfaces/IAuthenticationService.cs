namespace Equiparts.Interfaces;

// Login/logout orchestration used by ViewModels. Delegates token storage/refresh to
// ITokenService and calls IAuthApi directly - it never sees a Bearer header itself.
public interface IAuthenticationService
{
    Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    Task LogoutAsync();
}
