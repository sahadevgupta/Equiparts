namespace Equiparts.Interfaces;

// Reference-counted: nested Show() calls from independent operations only display a
// single overlay, and Hide() only actually dismisses it once every outstanding Show()
// has been matched - so one operation can never hide a popup another still needs.
public interface ILoadingPopupService
{
    /// <summary>Shows the blocking loading overlay. Dispose the result (or call Hide/HideAsync) to release this request.</summary>
    IDisposable Show();

    /// <summary>Same as <see cref="Show"/>, but returns once the overlay is actually visible.</summary>
    Task<IDisposable> ShowAsync();

    void Hide();

    /// <summary>Same as <see cref="Hide"/>, but returns once the overlay has actually been removed.</summary>
    Task HideAsync();
}
