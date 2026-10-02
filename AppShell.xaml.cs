using Equiparts.Interfaces;
using Equiparts.Views;

namespace Equiparts
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Pushed on top of the current tab rather than being a tab themselves,
            // so they need an explicit route registration for Shell.GoToAsync to find them.
            Routing.RegisterRoute(nameof(ProductPage), typeof(ProductPage));
            Routing.RegisterRoute(nameof(ProductDetailPage), typeof(ProductDetailPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(ChangePasswordPage), typeof(ChangePasswordPage));
            Routing.RegisterRoute(nameof(OrdersPage), typeof(OrdersPage));
            Routing.RegisterRoute(nameof(OrderDetailsPage), typeof(OrderDetailsPage));
            Routing.RegisterRoute(nameof(CheckoutPage), typeof(CheckoutPage));
        }
    }
}
