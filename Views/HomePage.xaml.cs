using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class HomePage : BasePage
{
	private readonly HomeViewModel _viewModel;

	public HomePage(HomeViewModel viewModel)
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