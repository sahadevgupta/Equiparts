using Equiparts.ViewModels;

namespace Equiparts.Extensions;

public static class ViewModelInitializer
{
    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<HomeViewModel>()
                        .AddTransient<CategoriesViewModel>()
                        .AddTransient<CartViewModel>()
                        .AddTransient<OrdersViewModel>()
                        .AddTransient<LoginViewModel>()
                        .AddTransient<ProductViewModel>()
                        .AddTransient<ProductDetailViewModel>();

        return builder;
    }
}
