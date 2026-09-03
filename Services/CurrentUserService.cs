using Equiparts.Interfaces;
using Equiparts.Models;

namespace Equiparts.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    public bool IsAuthenticated { get; private set; }

    public int? UserId { get; private set; }

    public string? FullName { get; private set; }

    public string? Email { get; private set; }

    public string? AccountType { get; private set; }

    public IReadOnlyList<string> Roles { get; private set; } = [];

    public void Set(UserSession session)
    {
        UserId = session.UserId;
        FullName = session.FullName;
        Email = session.Email;
        AccountType = session.AccountType;
        Roles = session.Roles.AsReadOnly();
        IsAuthenticated = true;
    }

    public void Clear()
    {
        UserId = null;
        FullName = null;
        Email = null;
        AccountType = null;
        Roles = [];
        IsAuthenticated = false;
    }
}
