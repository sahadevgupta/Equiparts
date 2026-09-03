using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class OrdersPage : BasePage
{
	private readonly OrdersViewModel _viewModel;
	public OrdersPage(OrdersViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = _viewModel = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		await _viewModel.LoadCommand.ExecuteAsync(null);
	}
}