using Equiparts.Interfaces;

namespace Equiparts.Services;

// The overlay itself is added directly to the native window/activity root by the
// platform-specific INativeLoadingOverlay injected below (see
// Platforms/*/Services/NativeLoadingOverlay.cs), rather than pushed through Shell/MAUI
// navigation: Shell's TabBar renders in its own native container that a page pushed as
// a modal does not reliably cover, so only a true native, top-of-window view guarantees
// the TabBar (and Flyout, and back navigation) are blocked underneath it. This class
// only owns the show/hide bookkeeping, which is identical on every platform.
public sealed class LoadingPopupService(INativeLoadingOverlay overlay) : ILoadingPopupService
{
    private readonly object _syncRoot = new();
    private int _activeCount;

    // Every display/dismiss is chained onto this so they always run strictly in order.
    // Without it, a Hide() landing while the overlay is still mid-show would find
    // nothing to remove, and a Show() landing while a previous overlay is still mid-hide
    // could add a second overlay on top of the first.
    private Task _pendingTransition = Task.CompletedTask;

    public IDisposable Show()
    {
        lock (_syncRoot)
        {
            if (++_activeCount == 1)
                _pendingTransition = Continue(_pendingTransition, DisplayAsync);
        }

        return new Releaser(this);
    }

    public async Task<IDisposable> ShowAsync()
    {
        var releaser = Show();

        Task pending;
        lock (_syncRoot)
        {
            pending = _pendingTransition;
        }

        await pending.ConfigureAwait(false);
        return releaser;
    }

    public void Hide() => _ = HideAsync();

    public Task HideAsync()
    {
        lock (_syncRoot)
        {
            if (_activeCount == 0)
                return Task.CompletedTask;

            if (--_activeCount == 0)
                _pendingTransition = Continue(_pendingTransition, DismissAsync);

            return _pendingTransition;
        }
    }

    private static async Task Continue(Task previous, Func<Task> next)
    {
        try
        {
            await previous.ConfigureAwait(false);
        }
        catch
        {
            // the previous transition already handled/swallowed its own failure
        }

        await next().ConfigureAwait(false);
    }

    private async Task DisplayAsync()
    {
        try
        {
            await overlay.ShowAsync().ConfigureAwait(false);
        }
        catch
        {
            // best-effort UI; never let a loader failure crash the caller
        }
    }

    private async Task DismissAsync()
    {
        try
        {
            await overlay.HideAsync().ConfigureAwait(false);
        }
        catch
        {
            // ignore - best-effort UI cleanup
        }
    }

    private sealed class Releaser(LoadingPopupService owner) : IDisposable
    {
        private int _disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
                owner.Hide();
        }
    }
}
