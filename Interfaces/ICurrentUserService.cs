using Equiparts.Models;

namespace Equiparts.Interfaces;

// Non-sensitive profile info for the signed-in user (no tokens). Lets ViewModels show
// "Hi, Admin" / check roles without ever touching ITokenService. Takes the UI-facing
// UserSession model, not the LoginResponse API DTO.
public interface ICurrentUserService
{
    bool IsAuthenticated { get; }

    int? UserId { get; }

    string? FullName { get; }

    string? Email { get; }

    string? AccountType { get; }

    IReadOnlyList<string> Roles { get; }

    void Set(UserSession session);

    void Clear();
}
