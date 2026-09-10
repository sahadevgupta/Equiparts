using CommunityToolkit.Maui;
using Equiparts.Controls;
using Equiparts.Extensions;
using Equiparts.Interfaces;
using FFImageLoading.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;

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
                .ConfigureMopups()
                .UseFFImageLoading()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                    fonts.AddFont("fa-solid-900.ttf", "FontAwesome");
                })
                .UseSentry(options =>
                {
                    // The DSN is the only required setting.
                    options.Dsn = "https://61510e07164ba569b9991ffe52ca1919@o4510259879673856.ingest.de.sentry.io/4512062900994128";

                    // Use debug mode if you want to see what the SDK is doing.
                    // Debug messages are written to stdout with Console.Writeline,
                    // and are viewable in your IDE's debug console or with 'adb logcat', etc.
                    // This option is not recommended when deploying your application.
                    options.Debug = true;

                    // Set TracesSampleRate to 1.0 to capture 100% of transactions for tracing.
                    // We recommend adjusting this value in production.
                    options.TracesSampleRate = 1.0;
                    // Enable logs to be sent to Sentry
                    options.EnableLogs = true;

                    // Other Sentry options can be set here.
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
