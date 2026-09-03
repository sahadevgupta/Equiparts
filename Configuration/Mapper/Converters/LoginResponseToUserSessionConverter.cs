using Equiparts.Models;
using Equiparts.Models.Auth;

namespace Equiparts.Configuration.Mapper.Converters;

public class LoginResponseToUserSessionConverter : ConverterBase<LoginResponse, UserSession>
{
    protected override UserSession ConvertImpl(LoginResponse source)
    {
        return new UserSession
        {
            UserId = source.UserId,
            FullName = source.FullName,
            Email = source.Email,
            AccountType = source.AccountType,
            Roles = [.. source.Roles]
        };
    }
}
