using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Interfaces;

namespace Equiparts.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILoadingPopupService _loadingPopupService;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    partial void OnErrorMessageChanged(string value) => OnPropertyChanged(nameof(HasError));

    public RegisterViewModel(IAuthenticationService authenticationService,
        ILoadingPopupService loadingPopupService)
    {
        Title = "Create Account";
        _authenticationService = authenticationService;
        _loadingPopupService = loadingPopupService;
    }

    [RelayCommand]
    private async Task GoToLogin() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy)
            return;

        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please fill in all fields.";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return;
        }

        IsBusy = true;

        using (_loadingPopupService.Show())
        {
            try
            {
                var (success, error) = await _authenticationService.RegisterAsync(FullName.Trim(), Email.Trim(), Password);

                if (!success)
                {
                    ErrorMessage = error ?? "Registration failed. Please try again.";
                    return;
                }

                Password = string.Empty;
                ConfirmPassword = string.Empty;

                await Shell.Current.GoToAsync("//app/home");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
