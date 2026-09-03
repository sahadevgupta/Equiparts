using Equiparts.Configuration;
using Equiparts.Handlers;
using Equiparts.Interfaces;
using Refit;

namespace Equiparts.Extensions;

public static class RefitClientInitializer
{
    public static MauiAppBuilder RegisterRefitClients(this MauiAppBuilder builder)
    {
        var baseAddress = new Uri(ApiConstants.BaseUrl);

        // AddRefitGeneratedClient (not AddRefitClient) - wires up Refit's compile-time
        // source-generated implementation instead of the reflection-based request
        // builder, which needs a separate Refit.Reflection package and isn't AOT/trim
        // friendly on mobile targets. Our interfaces are plain enough to fully
        // source-generate (no RF006 diagnostics).

        // Unauthenticated - must NOT go through AuthHandler or login/refresh would recurse.
        builder.Services.AddRefitGeneratedClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress);

        // Authenticated - AuthHandler attaches the bearer token and handles 401/refresh/retry.
        // Register future authenticated APIs (orders, catalog, etc.) the same way.
        builder.Services.AddRefitGeneratedClient<IUserApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress)
            .AddHttpMessageHandler<AuthHandler>();

        return builder;
    }
}
