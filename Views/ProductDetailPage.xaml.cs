using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class ProductDetailPage : BasePage
{
    private readonly ProductDetailViewModel _viewModel;

    public ProductDetailPage(ProductDetailViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }
}
