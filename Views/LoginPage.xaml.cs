using Equiparts.Controls;
using Equiparts.ViewModels;

namespace Equiparts.Views;

public partial class LoginPage : BasePage
{
    private bool _hasAnimatedIn;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        HeaderGraphics.Drawable = new CurvedHeaderDrawable();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_hasAnimatedIn)
            return;

        _hasAnimatedIn = true;
        _ = AnimateEntranceAsync();
    }

    // The wavy header and logo badge arrive already settled: when navigated
    // to from the splash screen, InitialLoadingActivityIndicator's own
    // transition already morphed the mark into this exact badge position/size
    // with the header grown in behind it, so replaying a separate fade/scale
    // here would look like a reset. Only the card still needs to slide up.
    private async Task AnimateEntranceAsync()
    {
        LoginCard.Opacity = 0;
        LoginCard.TranslationY = 56;

        var cardFade = LoginCard.FadeToAsync(1, 360, Easing.CubicOut);
        var cardSlide = LoginCard.TranslateToAsync(0, 0, 380, Easing.CubicOut);
        await Task.WhenAll(cardFade, cardSlide);
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
