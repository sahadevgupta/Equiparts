namespace Equiparts.Models;

// UI-facing model for IUserApi.GetProfileAsync - mapped from the UserProfileResponse
// API DTO via UserProfileResponseToUserProfileConverter.
public class UserProfile
{
    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string AccountType { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = [];
}
