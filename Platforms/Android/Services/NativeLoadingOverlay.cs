using Android.Views;
using AndroidX.Activity;
using Equiparts.Interfaces;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Platform;
using Color = Microsoft.Maui.Graphics.Color;
using FrameLayout = Android.Widget.FrameLayout;
using Image = Microsoft.Maui.Controls.Image;
using View = Android.Views.View;

namespace Equiparts.Platforms.Android.Services;

public sealed class NativeLoadingOverlay : INativeLoadingOverlay
{
    private View? _overlay;
    private Image? _logo;
    private OnBackPressedCallback? _backCallback;

    public Task ShowAsync()
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
        {
            var activity = Platform.CurrentActivity;
            var mauiContext = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page?.Handler?.MauiContext;
            if (activity?.Window?.DecorView is not ViewGroup decorView || mauiContext is null)
                return;

            var overlay = new FrameLayout(activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
                Clickable = true,
                Focusable = true,
                FocusableInTouchMode = true,
            };
            overlay.SetBackgroundColor(ScrimColor.ToPlatform());

            var logo = new Image
            {
                Source = "ep_logo_animate.gif",
                IsAnimationPlaying = true,
            };

            if (logo.ToHandler(mauiContext).PlatformView is View logoPlatformView)
            {
                var density = activity.Resources?.DisplayMetrics?.Density ?? 1f;
                var sizePx = (int)(96 * density);
                var logoParams = new FrameLayout.LayoutParams(sizePx, sizePx)
                {
                    Gravity = GravityFlags.Center,
                };
                overlay.AddView(logoPlatformView, logoParams);
            }

            decorView.AddView(overlay);
            _overlay = overlay;
            _logo = logo;

            if (activity is ComponentActivity componentActivity)
            {
                var callback = new BlockingBackPressedCallback();
                componentActivity.OnBackPressedDispatcher.AddCallback(callback);
                _backCallback = callback;
            }
        });
    }

    public Task HideAsync()
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
        {
            _backCallback?.Remove();
            _backCallback = null;

            // Stop the GIF before detaching - see the note above.
            if (_logo is not null)
                _logo.IsAnimationPlaying = false;
            _logo = null;

            if (_overlay?.Parent is ViewGroup parent)
                parent.RemoveView(_overlay);
            _overlay = null;
        });
    }

    private static Color ScrimColor => Color.FromArgb("#A0000000");

    private sealed class BlockingBackPressedCallback() : OnBackPressedCallback(true)
    {
        public override void HandleOnBackPressed()
        {
            // swallow - navigation is blocked while the loading overlay is visible
        }
    }
}
