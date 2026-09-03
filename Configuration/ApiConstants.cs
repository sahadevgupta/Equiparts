namespace Equiparts.Configuration;

public static class ApiConstants
{
    // TODO: confirm the production base URL with the backend team.
    public const string BaseUrl = "https://api.equipartsindia.com";

    public static readonly TimeSpan AccessTokenExpiryBuffer = TimeSpan.FromMinutes(1);
}
