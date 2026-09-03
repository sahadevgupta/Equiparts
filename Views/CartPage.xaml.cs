using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class CartPage : BasePage
{
	public CartPage(CartViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}