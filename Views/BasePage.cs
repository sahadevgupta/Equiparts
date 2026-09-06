using Equiparts.Controls;
using Equiparts.ViewModels;

namespace Equiparts.Views;

// Shared page chrome: every page gets the offline banner for free by inheriting from
// this instead of ContentPage, rather than dropping <controls:OfflineBannerView/> into
// each page's XAML individually. Page content is set via the PageContent property
// element (<views:BasePage.PageContent>...</views:BasePage.PageContent>) instead of
// ContentPage's plain Content, so the banner row can sit above it.
public abstract class BasePage : ContentPage
{
    public static readonly BindableProperty PageContentProperty =
        BindableProperty.Create(nameof(PageContent), typeof(View), typeof(BasePage), propertyChanged: OnPageContentChanged);

    private readonly ContentView _pageContentHost;

    protected BasePage()
    {
        var layout = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star }
            }
        };

        var offlineBanner = new OfflineBannerView();
        layout.Add(offlineBanner);
        Grid.SetRow(offlineBanner, 0);

        _pageContentHost = new ContentView();
        layout.Add(_pageContentHost);
        Grid.SetRow(_pageContentHost, 1);

        Content = layout;
    }

    public View PageContent
    {
        get => (View)GetValue(PageContentProperty);
        set => SetValue(PageContentProperty, value);
    }

    private static void OnPageContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BasePage basePage && newValue is View newContent)
            basePage._pageContentHost.Content = newContent;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (BindingContext is BaseViewModel viewModel && (args.NavigationType is NavigationType.Push or NavigationType.Replace))
        {
            viewModel.LoadDataOnNavigatedTo();
        }
    }
}
