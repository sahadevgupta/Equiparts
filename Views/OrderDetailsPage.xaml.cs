using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class OrderDetailsPage : BasePage
{
    private readonly OrderDetailsViewModel _viewModel;

    public OrderDetailsPage(OrderDetailsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }
}
