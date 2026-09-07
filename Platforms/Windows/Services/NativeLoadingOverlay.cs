using Equiparts.Interfaces;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Platform;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Image = Microsoft.Maui.Controls.Image;

namespace Equiparts.Platforms.Windows.Services;

// A WinUI Popup sized to the whole XamlRoot: it consumes every pointer input within
// its (full-window) bounds, so nothing behind it - including the Shell TabBar - is
// reachable while it's open.
public sealed class NativeLoadingOverlay : INativeLoadingOverlay
{
    // NOTE: the loading visual is ep_logo_animate.gif via a real Microsoft.Maui.Controls
    // .Image with IsAnimationPlaying=true - the same combination already used on
    // LoginPage - converted to its native PlatformView, rather than a plain
    // ProgressRing. Unverified on-device (this project's Windows target only compiles
    // on a Windows host); any runtime failure here is swallowed by
    // LoadingPopupService's own try/catch.
    private Popup? _overlay;
    private Image? _logo;

    public Task ShowAsync()
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
        {
            var mauiWindow = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault();
            var mauiContext = mauiWindow?.Page?.Handler?.MauiContext;
            var xamlRoot = (mauiWindow?.Handler?.PlatformView as Window)?.Content?.XamlRoot;
            if (xamlRoot is null || mauiContext is null)
                return;

            var size = xamlRoot.Size;

            var content = new Grid
            {
                Width = size.Width,
                Height = size.Height,
                Background = new SolidColorBrush(ColorHelper.FromArgb(179, 8, 31, 62)),
            };

            var logo = new Image
            {
                Source = "ep_logo_animate.gif",
                IsAnimationPlaying = true,
            };

            if (logo.ToHandler(mauiContext).PlatformView is FrameworkElement logoPlatformView)
            {
                logoPlatformView.Width = 96;
                logoPlatformView.Height = 96;
                logoPlatformView.HorizontalAlignment = HorizontalAlignment.Center;
                logoPlatformView.VerticalAlignment = VerticalAlignment.Center;
                content.Children.Add(logoPlatformView);
            }

            var popup = new Popup
            {
                XamlRoot = xamlRoot,
                IsLightDismissEnabled = false,
                Child = content,
            };
            popup.IsOpen = true;
            _overlay = popup;
            _logo = logo;
        });
    }

    public Task HideAsync()
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (_logo is not null)
                _logo.IsAnimationPlaying = false;
            _logo = null;

            if (_overlay is not null)
                _overlay.IsOpen = false;
            _overlay = null;
        });
    }
}
