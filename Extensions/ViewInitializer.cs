namespace Equiparts.Extensions;

public static class ViewInitializer
{
    public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        // Resolved once via DI in App.CreateWindow, so it needs an explicit registration.
        builder.Services.AddSingleton<AppShell>();

        return builder;
    }
}
