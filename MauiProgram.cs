using CommunityToolkit.Maui;
using Equiparts.Interfaces;
using Equiparts.Services;
using Equiparts.ViewModels;
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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                    fonts.AddFont("fa-solid-900.ttf", "FontAwesome");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddTransient<HomeViewModel>()
                            .AddTransient<CategoriesViewModel>()
                            .AddTransient<CartViewModel>()
                            .AddTransient<OrdersViewModel>();

            builder.Services.AddSingleton<ICartService, CartService>();
            builder.Services.AddSingleton<IProductService, ProductService>();
            builder.Services.AddSingleton<IOrderService, OrderService>();

            builder.ConfigureMauiHandlers(handlers =>
            {
                //handlers.AddHandler<Microsoft.Maui.Controls.CarouselView, Microsoft.Maui.Controls.Handlers.Items.CarouselViewHandler>();
#if ANDROID
                handlers.AddHandler(typeof(Shell), typeof(Equiparts.Platforms.Android.Handlers.CustomShellRenderer));
#endif
            });

            return builder.Build();
        }
    }
}
