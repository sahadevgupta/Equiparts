using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class LoginPage : BasePage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
