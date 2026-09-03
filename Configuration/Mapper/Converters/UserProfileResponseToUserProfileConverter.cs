using Equiparts.Models;
using Equiparts.Models.Auth;

namespace Equiparts.Configuration.Mapper.Converters;

public class UserProfileResponseToUserProfileConverter : ConverterBase<UserProfileResponse, UserProfile>
{
    protected override UserProfile ConvertImpl(UserProfileResponse source)
    {
        return new UserProfile
        {
            UserId = source.UserId,
            FullName = source.FullName,
            Email = source.Email,
            AccountType = source.AccountType,
            Roles = [.. source.Roles]
        };
    }
}
