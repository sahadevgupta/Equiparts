using Equiparts.Controls;
using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class RegisterPage : BasePage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        HeaderGraphics.Drawable = new CurvedHeaderDrawable();
    }
}
