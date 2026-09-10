using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class ChangePasswordPage : BasePage
{
    public ChangePasswordPage(ChangePasswordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
