using CommunityToolkit.Maui;
using Equiparts.Handlers;
using Equiparts.Interfaces;
using Equiparts.Services;

namespace Equiparts.Extensions;

public static class AppServiceInitializer
{
    public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<AuthHandler>()
                        .AddTransient<INavigationService, NavigationService>();

        builder.Services.AddTransient<ICartService, CartService>()
                        .AddTransient<IProductService, ProductService>()
                        .AddTransient<IOrderService, OrderService>()
                        .AddTransient<IProfileService, ProfileService>();

        // Auth: token storage/refresh + connectivity.
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current)
                        .AddSingleton<IConnectivityService, ConnectivityService>()
                        .AddSingleton<ITokenService, TokenService>()
                        .AddSingleton<ICurrentUserService, CurrentUserService>()
                        .AddSingleton<IAuthenticationService, AuthenticationService>()
                        .AddSingleton<IAppLifeCycleCoordinator, AppLifeCycleCoordinator>();

        // The native overlay behind the loading popup is platform-specific (it's added
        // directly to the platform window root - see Platforms/*/Services/
        // NativeLoadingOverlay.cs) so only one implementation is ever registered per build.
#if ANDROID
        builder.Services.AddSingleton<INativeLoadingOverlay, Equiparts.Platforms.Android.Services.NativeLoadingOverlay>();
#elif IOS
        builder.Services.AddSingleton<INativeLoadingOverlay, Equiparts.Platforms.iOS.Services.NativeLoadingOverlay>();
#elif MACCATALYST
        builder.Services.AddSingleton<INativeLoadingOverlay, Equiparts.Platforms.MacCatalyst.Services.NativeLoadingOverlay>();
#elif WINDOWS
        builder.Services.AddSingleton<INativeLoadingOverlay, Equiparts.Platforms.Windows.Services.NativeLoadingOverlay>();
#endif
        builder.Services.AddSingleton<ILoadingPopupService, LoadingPopupService>();

        builder.Services.AddSingleton(sp =>
            new Lazy<IAppLifeCycleCoordinator>(() => sp.GetRequiredService<IAppLifeCycleCoordinator>()));

        return builder;
    }
}
