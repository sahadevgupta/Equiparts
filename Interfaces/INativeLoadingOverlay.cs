namespace Equiparts.Interfaces;

// The native, platform-specific mechanics behind ILoadingPopupService: adding/removing
// a view at the platform window root so it sits above Shell's TabBar and Flyout. Kept
// separate from LoadingPopupService (and injected via DI, one concrete type per
// platform - see Platforms/*/Services/NativeLoadingOverlay.cs) so the reference-counted
// show/hide bookkeeping, which is identical on every platform, isn't duplicated.
public interface INativeLoadingOverlay
{
    Task ShowAsync();

    Task HideAsync();
}
