using Equiparts.Models;
using Equiparts.Models.Profile;

namespace Equiparts.Configuration.Mapper.Converters;

public class ProfileResponseToUserProfileConverter : ConverterBase<ProfileResponse, UserProfile>
{
    protected override UserProfile ConvertImpl(ProfileResponse source)
    {
        return new UserProfile
        {
            UserId = source.UserId,
            FullName = source.FullName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            AccountType = source.AccountType,
            Roles = [.. source.Roles]
        };
    }
}
