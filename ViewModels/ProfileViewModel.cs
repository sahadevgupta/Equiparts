using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Interfaces;
using Equiparts.Views;

namespace Equiparts.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IProfileService _profileService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthenticationService _authenticationService;

    [ObservableProperty]
    string initials = string.Empty;

    [ObservableProperty]
    string fullName = string.Empty;

    [ObservableProperty]
    string email = string.Empty;

    [ObservableProperty]
    string accountType = string.Empty;

    [ObservableProperty]
    string userIdLabel = string.Empty;

    [ObservableProperty]
    bool marketingUpdatesEnabled = true;

    [ObservableProperty]
    bool appNotificationsEnabled = true;

    public string AppVersionLabel { get; } =
        $"Equiparts v{AppInfo.Current.VersionString} (Build {AppInfo.Current.BuildString})";

    public ProfileViewModel(
        IProfileService profileService,
        ICurrentUserService currentUserService,
        IAuthenticationService authenticationService)
    {
        Title = "Profile";

        _profileService = profileService;
        _currentUserService = currentUserService;
        _authenticationService = authenticationService;

        ApplyCurrentUser();
    }

    public override void LoadDataOnNavigatedTo() => _ = Load();

    [RelayCommand]
    async Task Load()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        // Show cached session data immediately, then refresh from the server.
        ApplyCurrentUser();

        var profile = await _profileService.GetProfileAsync();
        if (profile is not null)
        {
            FullName = profile.FullName;
            Email = profile.Email;
            AccountType = profile.AccountType;
            UserIdLabel = $"ID: #{profile.UserId:D4}";
            Initials = GetInitials(profile.FullName);
        }

        IsBusy = false;
    }

    void ApplyCurrentUser()
    {
        FullName = _currentUserService.FullName ?? string.Empty;
        Email = _currentUserService.Email ?? string.Empty;
        AccountType = _currentUserService.AccountType ?? string.Empty;
        UserIdLabel = _currentUserService.UserId is int id ? $"ID: #{id:D4}" : string.Empty;
        Initials = GetInitials(FullName);
    }

    static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return parts.Length switch
        {
            0 => string.Empty,
            1 => parts[0][..Math.Min(2, parts[0].Length)].ToUpperInvariant(),
            _ => $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant()
        };
    }

    [RelayCommand]
    async Task GoToOrders() => await Shell.Current.GoToAsync("//orders");

    [RelayCommand]
    async Task GoToAddresses() =>
        await Shell.Current.DisplayAlertAsync("Delivery Addresses", "Address management is coming soon.", "OK");

    [RelayCommand]
    async Task GoToWishlist() =>
        await Shell.Current.DisplayAlertAsync("Saved Items", "Your wishlist is coming soon.", "OK");

    [RelayCommand]
    async Task GoToChangePassword() => await Shell.Current.GoToAsync(nameof(ChangePasswordPage));

    [RelayCommand]
    async Task GoToHelp() =>
        await Shell.Current.DisplayAlertAsync("Help & Support", "Support contact details are coming soon.", "OK");

    [RelayCommand]
    async Task GoToTerms() =>
        await Shell.Current.DisplayAlertAsync("Terms & Privacy", "Policies and user agreements are coming soon.", "OK");

    [RelayCommand]
    async Task Logout()
    {
        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Log Out",
            "Are you sure you want to log out?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _authenticationService.LogoutAsync();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}
