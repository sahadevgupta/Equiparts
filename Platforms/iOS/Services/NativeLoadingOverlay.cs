using CoreGraphics;
using Equiparts.Interfaces;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Platform;
using UIKit;
using Color = Microsoft.Maui.Graphics.Color;
using Image = Microsoft.Maui.Controls.Image;

namespace Equiparts.Platforms.iOS.Services;

// Added directly to the key UIWindow itself - not the root view controller's view -
// so it sits above everything currently on screen: tab bar, navigation bar, and any
// presented view controller, regardless of what Shell has on screen. A UIView placed
// on top naturally consumes every touch within its bounds, so the tab bar, nav-bar
// back button, and the edge-swipe pop gesture underneath all become unreachable
// without any extra gesture-recognizer wiring.
public sealed class NativeLoadingOverlay : INativeLoadingOverlay
{
    // NOTE: the loading visual is ep_logo_animate.gif via a real Microsoft.Maui.Controls
    // .Image with IsAnimationPlaying=true - the same combination already used (and
    // proven safe) on LoginPage - converted to its native PlatformView and re-parented
    // here, rather than a plain UIActivityIndicatorView. Unlike Android's
    // AnimatedImageDrawable, iOS's GIF-frame cycling runs on the main run loop rather
    // than a background decode thread, so it doesn't carry the same removal-crash risk
    // AirIQ hit on Android - but IsAnimationPlaying is still set false before detaching
    // below for a clean, symmetric teardown.
    private UIView? _overlay;
    private Image? _logo;

    public Task ShowAsync()
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
        {
            var window = UIApplication.SharedApplication.ConnectedScenes
                .OfType<UIWindowScene>()
                .SelectMany(scene => scene.Windows)
                .FirstOrDefault(w => w.IsKeyWindow);

            var mauiContext = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page?.Handler?.MauiContext;
            if (window is null || mauiContext is null)
                return;

            var overlay = new UIView(window.Bounds)
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                BackgroundColor = ScrimColor.ToPlatform(),
                UserInteractionEnabled = true,
            };

            var logo = new Image
            {
                Source = "ep_logo_animate.gif",
                IsAnimationPlaying = true,
            };

            if (logo.ToHandler(mauiContext).PlatformView is UIView logoPlatformView)
            {
                nfloat size = 96;
                logoPlatformView.Frame = new CGRect(0, 0, size, size);
                logoPlatformView.Center = new CGPoint(overlay.Bounds.GetMidX(), overlay.Bounds.GetMidY());
                logoPlatformView.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
                overlay.AddSubview(logoPlatformView);
            }

            window.AddSubview(overlay);
            _overlay = overlay;
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

            _overlay?.RemoveFromSuperview();
            _overlay = null;
        });
    }

    private static Color ScrimColor => Color.FromArgb("#B31A1A1A");
}
