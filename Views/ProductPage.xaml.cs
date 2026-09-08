using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class ProductPage : BasePage
{
    private readonly ProductViewModel _viewModel;

    public ProductPage(ProductViewModel viewModel)
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
