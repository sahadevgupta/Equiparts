using CommunityToolkit.Maui;
using Equiparts.Controls;
using Equiparts.Extensions;
using Equiparts.Interfaces;
using FFImageLoading.Maui;
using Microsoft.Extensions.Logging;

namespace Equiparts
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseFFImageLoading()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                    fonts.AddFont("fa-solid-900.ttf", "FontAwesome");
                })
                .RegisterAppServices()
                .RegisterViewModels()
                .RegisterViews()
                .RegisterRefitClients();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.ConfigureMauiHandlers(handlers =>
            {
                //handlers.AddHandler<Microsoft.Maui.Controls.CarouselView, Microsoft.Maui.Controls.Handlers.Items.CarouselViewHandler>();
#if ANDROID
                //handlers.AddHandler(typeof(Shell), typeof(Equiparts.Platforms.Android.Handlers.CustomShellRenderer));
#endif
                handlers.AddHandler<BorderlessEntry, Equiparts.Platforms.Handlers.PlainEntryHandler>();
            });

            var app = builder.Build();

            // Warm up the connectivity service immediately so the offline banner and
            // token-refresh gating are accurate from the very first screen.
            app.Services.GetRequiredService<IConnectivityService>();

            return app;
        }
    }
}
