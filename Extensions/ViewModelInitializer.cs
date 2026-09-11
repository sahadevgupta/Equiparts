using Equiparts.ViewModels;

namespace Equiparts.Extensions;

public static class ViewModelInitializer
{
    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<HomeViewModel>()
                        .AddTransient<CategoriesViewModel>()
                        .AddTransient<CartViewModel>()
                        .AddTransient<CheckoutViewModel>()
                        .AddTransient<OrdersViewModel>()
                        .AddTransient<OrderDetailsViewModel>()
                        .AddTransient<LoginViewModel>()
                        .AddTransient<RegisterViewModel>()
                        .AddTransient<ProductViewModel>()
                        .AddTransient<ProductDetailViewModel>()
                        .AddTransient<ProfileViewModel>()
                        .AddTransient<ChangePasswordViewModel>();

        return builder;
    }
}
