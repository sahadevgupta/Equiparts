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

        builder.Services.AddSingleton<ICartService, CartService>()
                        .AddSingleton<IProductService, ProductService>()
                        .AddSingleton<IOrderService, OrderService>();

        // Auth: token storage/refresh + connectivity.
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current)
                        .AddSingleton<IConnectivityService, ConnectivityService>()
                        .AddSingleton<ITokenService, TokenService>()
                        .AddSingleton<ICurrentUserService, CurrentUserService>()
                        .AddSingleton<IAuthenticationService, AuthenticationService>()
                        .AddSingleton<IAppLifeCycleCoordinator, AppLifeCycleCoordinator>();

        builder.Services.AddSingleton(sp =>
            new Lazy<IAppLifeCycleCoordinator>(() => sp.GetRequiredService<IAppLifeCycleCoordinator>()));

        return builder;
    }
}
