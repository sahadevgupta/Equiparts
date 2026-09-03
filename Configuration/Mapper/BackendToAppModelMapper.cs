using Equiparts.Configuration.Mapper.Converters;
using Equiparts.Models;
using Equiparts.Models.Auth;

namespace Equiparts.Configuration.Mapper;

public static class BackendToAppModelMapper
{
    public static UserSession? GetUserSession(LoginResponse? loginResponse)
    {
        if (loginResponse is null)
            return null;

        var converter = new LoginResponseToUserSessionConverter();
        return converter.Convert(loginResponse);
    }

    public static UserProfile? GetUserProfile(UserProfileResponse? userProfileResponse)
    {
        if (userProfileResponse is null)
            return null;

        var converter = new UserProfileResponseToUserProfileConverter();
        return converter.Convert(userProfileResponse);
    }
}
