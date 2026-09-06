namespace Equiparts.Models;

// UI-facing model for IProfileApi.GetProfileAsync - mapped from the ProfileResponse
// API DTO via ProfileResponseToUserProfileConverter.
public class UserProfile
{
    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string AccountType { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = [];
}
