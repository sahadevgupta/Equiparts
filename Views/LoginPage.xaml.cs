using Equiparts.Controls;
using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class LoginPage : BasePage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        HeaderGraphics.Drawable = new CurvedHeaderDrawable();
    }

    // Entry has no bindable border, so the surrounding Border's stroke color is
    // toggled here directly to mimic the focused-field highlight from the design.
    private void OnFieldFocused(object sender, FocusEventArgs e)
    {
        if (sender is Entry { Parent: Grid { Parent: Border border } })
            border.Stroke = (Color)Application.Current!.Resources["LinkColor"];
    }

    private void OnFieldUnfocused(object sender, FocusEventArgs e)
    {
        if (sender is Entry { Parent: Grid { Parent: Border border } })
            border.Stroke = (Color)Application.Current!.Resources["Border"];
    }
}
