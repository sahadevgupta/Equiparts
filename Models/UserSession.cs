namespace Equiparts.Models;

// UI-facing model for the signed-in user - never carries tokens. Mapped from the
// LoginResponse API DTO via LoginResponseToUserSessionConverter.
public class UserSession
{
    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string AccountType { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = [];
}
