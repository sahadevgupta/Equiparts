using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Controls;
using Equiparts.Enums;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Views;
using Mopups.Interfaces;

namespace Equiparts.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IProfileService _profileService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IDialogService _dialogService;
    private readonly ILoadingPopupService _loadingPopupService;
    private readonly IPopupNavigation _popupNavigation;

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
        IAuthenticationService authenticationService,
        IDialogService dialogService,
        ILoadingPopupService loadingPopupService,
        IPopupNavigation popupNavigation)
    {
        Title = "Profile";

        _profileService = profileService;
        _currentUserService = currentUserService;
        _authenticationService = authenticationService;
        _dialogService = dialogService;
        _loadingPopupService = loadingPopupService;
        _popupNavigation = popupNavigation;

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
    async Task GoToOrders() => await Shell.Current.GoToAsync(nameof(OrdersPage));

    [RelayCommand]
    async Task GoToAddresses()
    {
        List<Address> addresses;

        using (_loadingPopupService.Show())
        {
            addresses = await _profileService.GetAddressesAsync();
        }

        await ShowAddressManagerAsync(addresses);
    }

    private async Task ShowAddressManagerAsync(List<Address> addresses)
    {
        var selectedId = addresses.FirstOrDefault(a => a.IsDefault)?.Id ?? 0;

        var popup = new AddressSelectorPopup(addresses, selectedId, "Manage Addresses");
        await _popupNavigation.PushAsync(popup);
        var selection = await popup.Result;

        if (selection is null)
            return;

        if (selection.SelectedAddress is not null)
        {
            await SetDefaultAddressAsync(selection.SelectedAddress);
            return;
        }

        if (selection.IsAddNew)
        {
            await OpenAddressEditorAsync(null);
            return;
        }

        if (selection.EditAddress is not null)
            await OpenAddressEditorAsync(selection.EditAddress);
    }

    private async Task SetDefaultAddressAsync(Address address)
    {
        List<Address> refreshed;

        using (_loadingPopupService.Show())
        {
            if (!address.IsDefault)
            {
                address.IsDefault = true;
                await _profileService.UpdateAddressAsync(address.Id, address);
            }

            refreshed = await _profileService.GetAddressesAsync();
        }

        await ShowAddressManagerAsync(refreshed);
    }

    private async Task OpenAddressEditorAsync(Address? existing)
    {
        var popup = new AddressEditorPopup(existing);
        await _popupNavigation.PushAsync(popup);
        var formAddress = await popup.Result;

        List<Address> refreshed;

        using (_loadingPopupService.Show())
        {
            if (formAddress is not null)
            {
                var saved = existing is null
                    ? await _profileService.AddAddressAsync(formAddress)
                    : await _profileService.UpdateAddressAsync(existing.Id, formAddress);

                if (saved is null)
                    await _dialogService.ShowAlertAsync("Failed to save address. Please try again.", AlertType.Error);
            }

            refreshed = await _profileService.GetAddressesAsync();
        }

        await ShowAddressManagerAsync(refreshed);
    }

    [RelayCommand]
    async Task GoToWishlist() =>
        await _dialogService.ShowAlertAsync("Your wishlist is coming soon.");

    [RelayCommand]
    async Task GoToChangePassword() => await Shell.Current.GoToAsync(nameof(ChangePasswordPage));

    [RelayCommand]
    async Task GoToHelp() =>
        await _dialogService.ShowAlertAsync("Support contact details are coming soon.");

    [RelayCommand]
    async Task GoToTerms() =>
        await _dialogService.ShowAlertAsync("Policies and user agreements are coming soon.");

    [RelayCommand]
    async Task Logout()
    {
        bool confirm = await _dialogService.ShowConfirmAsync(
            "Are you sure you want to log out?",
            "Yes",
            "No");

        if (!confirm)
            return;

        using (_loadingPopupService.Show())
        {
            await _authenticationService.LogoutAsync();
        }

        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}
