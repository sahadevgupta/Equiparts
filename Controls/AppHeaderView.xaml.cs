using System.Windows.Input;

namespace Equiparts.Controls;

// Curved brand header (logo, title/subtitle, notification bell, optional search bar)
// shared across pages - originally built inline on HomePage. Any page that wraps its
// content the way HomePage does can drop this in at the top of its own Grid.
public partial class AppHeaderView : ContentView
{
    const string supportNumber = "+919733011102";
    public AppHeaderView()
    {
        InitializeComponent();

        HeaderGraphics.Drawable = new CurvedHeaderDrawable();
    }

    public static readonly BindableProperty HeaderTitleProperty =
        BindableProperty.Create(nameof(HeaderTitle), typeof(string), typeof(AppHeaderView), "EQUIPARTS");

    public string HeaderTitle
    {
        get => (string)GetValue(HeaderTitleProperty);
        set => SetValue(HeaderTitleProperty, value);
    }

    public static readonly BindableProperty HeaderSubtitleProperty =
        BindableProperty.Create(nameof(HeaderSubtitle), typeof(string), typeof(AppHeaderView), "Genuine - OEM - Aftermarket");

    public string HeaderSubtitle
    {
        get => (string)GetValue(HeaderSubtitleProperty);
        set => SetValue(HeaderSubtitleProperty, value);
    }

    public static readonly BindableProperty ShowSearchBarProperty =
        BindableProperty.Create(nameof(ShowSearchBar), typeof(bool), typeof(AppHeaderView), true);

    public bool ShowSearchBar
    {
        get => (bool)GetValue(ShowSearchBarProperty);
        set => SetValue(ShowSearchBarProperty, value);
    }

    public static readonly BindableProperty SearchPlaceholderProperty =
        BindableProperty.Create(nameof(SearchPlaceholder), typeof(string), typeof(AppHeaderView), "Search parts, SKU, brand...");

    public string SearchPlaceholder
    {
        get => (string)GetValue(SearchPlaceholderProperty);
        set => SetValue(SearchPlaceholderProperty, value);
    }

    public static readonly BindableProperty SearchQueryProperty =
        BindableProperty.Create(nameof(SearchQuery), typeof(string), typeof(AppHeaderView), string.Empty, BindingMode.TwoWay);

    public string SearchQuery
    {
        get => (string)GetValue(SearchQueryProperty);
        set => SetValue(SearchQueryProperty, value);
    }

    // Fires on the search icon tap and on the entry's return key. Also backs the
    // notification bell below - both are simply no-ops if the consuming page
    // doesn't set them.
    public static readonly BindableProperty SearchCommandProperty =
        BindableProperty.Create(nameof(SearchCommand), typeof(ICommand), typeof(AppHeaderView));

    public ICommand SearchCommand
    {
        get => (ICommand)GetValue(SearchCommandProperty);
        set => SetValue(SearchCommandProperty, value);
    }

    public static readonly BindableProperty NotificationCommandProperty =
        BindableProperty.Create(nameof(NotificationCommand), typeof(ICommand), typeof(AppHeaderView));

    public ICommand NotificationCommand
    {
        get => (ICommand)GetValue(NotificationCommandProperty);
        set => SetValue(NotificationCommandProperty, value);
    }

    public static readonly BindableProperty MainViewHeightProperty =
        BindableProperty.Create(nameof(MainViewHeight), typeof(double), typeof(AppHeaderView), 180.0);

    public double MainViewHeight
    {
        get => (double)GetValue(MainViewHeightProperty);
        set => SetValue(MainViewHeightProperty, value);
    }

    public static readonly BindableProperty CurveHeightProperty =
        BindableProperty.Create(nameof(CurveHeight), typeof(double), typeof(AppHeaderView), 212.0);

    public double CurveHeight
    {
        get => (double)GetValue(CurveHeightProperty);
        set => SetValue(CurveHeightProperty, value);
    }

    private async void QuickSupport_Tapped(object sender, TappedEventArgs e)
    {
        await OpenWhatsAppAsync();
    }

    private async Task OpenWhatsAppAsync()
    {
        var whatsappUrl = $"https://wa.me/{supportNumber}";

        try
        {
            await Launcher.Default.OpenAsync(whatsappUrl);
        }
        catch (Exception ex)
        {
            // Handle WhatsApp/browser unavailable
        }
    }
}
