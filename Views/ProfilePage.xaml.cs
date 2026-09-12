using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class ProfilePage : BasePage
{
    private readonly ProfileViewModel _viewModel;

    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }
}
