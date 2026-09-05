using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Extensions;
using Equiparts.Interfaces;

namespace Equiparts.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authenticationService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isPasswordHidden = true;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public string PasswordToggleIcon => IsPasswordHidden
        ? FontAwesomeIcons.EyeSlash
        : FontAwesomeIcons.Eye;

    partial void OnErrorMessageChanged(string value) => OnPropertyChanged(nameof(HasError));

    partial void OnIsPasswordHiddenChanged(bool value) => OnPropertyChanged(nameof(PasswordToggleIcon));

    public LoginViewModel(IAuthenticationService authenticationService)
    {
        Title = "Sign In";
        _authenticationService = authenticationService;
    }

    [RelayCommand]
    private void TogglePasswordVisibility() => IsPasswordHidden = !IsPasswordHidden;

    [RelayCommand]
    private void ForgotPassword() => ErrorMessage = "Password reset isn't available yet. Please contact support.";

    [RelayCommand]
    private void ContactSupport() => ErrorMessage = "Please reach out to your Equiparts Groups administrator for support.";

    [RelayCommand]
    private void LoginWithBiometrics() => ErrorMessage = "Biometric login isn't available yet.";

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter both email and password.";
            return;
        }

        IsBusy = true;

        try
        {
            var (success, error) = await _authenticationService.LoginAsync(Email.Trim(), Password);

            if (!success)
            {
                ErrorMessage = error ?? "Login failed. Please try again.";
                return;
            }

            Password = string.Empty;

            // LoginPage was pushed on top of whatever page was showing (via AppShell's
            // "/LoginPage" global-route navigation), so pop back to it rather than
            // resetting the whole Shell stack.
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
