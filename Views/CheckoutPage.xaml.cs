using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class CheckoutPage : BasePage
{
    public CheckoutPage(CheckoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
