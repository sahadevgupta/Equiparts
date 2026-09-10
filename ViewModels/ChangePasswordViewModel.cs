using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Interfaces;

namespace Equiparts.ViewModels;

public partial class ChangePasswordViewModel : BaseViewModel
{
    private readonly IProfileService _profileService;
    private readonly ILoadingPopupService _loadingPopupService;

    [ObservableProperty]
    private string _currentPassword = string.Empty;

    [ObservableProperty]
    private string _newPassword = string.Empty;

    [ObservableProperty]
    private string _confirmNewPassword = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public bool HasSuccess => !string.IsNullOrEmpty(SuccessMessage);

    partial void OnErrorMessageChanged(string value) => OnPropertyChanged(nameof(HasError));

    partial void OnSuccessMessageChanged(string value) => OnPropertyChanged(nameof(HasSuccess));

    public ChangePasswordViewModel(IProfileService profileService,
        ILoadingPopupService loadingPopupService)
    {
        Title = "Change Password";
        _profileService = profileService;
        _loadingPopupService = loadingPopupService;
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        if (IsBusy)
            return;

        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
        {
            ErrorMessage = "Please fill in all fields.";
            return;
        }

        if (NewPassword != ConfirmNewPassword)
        {
            ErrorMessage = "New passwords do not match.";
            return;
        }

        if (NewPassword == CurrentPassword)
        {
            ErrorMessage = "New password must be different from the current password.";
            return;
        }

        IsBusy = true;

        using (_loadingPopupService.Show())
        {
            try
            {
                var (success, error) = await _profileService.ChangePasswordAsync(CurrentPassword, NewPassword);

                if (!success)
                {
                    ErrorMessage = error ?? "Failed to change password. Please try again.";
                    return;
                }

                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;
                SuccessMessage = "Your password has been updated successfully.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
