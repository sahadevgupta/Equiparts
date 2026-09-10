using Equiparts.Interfaces;
using Equiparts.Views;

namespace Equiparts
{
    public partial class AppShell : Shell
    {
        private readonly ITokenService _tokenService;

        public AppShell()
        {
            InitializeComponent();

            //_tokenService = tokenService;
            // _tokenService.SessionExpired += OnSessionExpired;

            // Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));

            // Pushed on top of the current tab rather than being a tab themselves,
            // so they need an explicit route registration for Shell.GoToAsync to find them.
            Routing.RegisterRoute(nameof(ProductPage), typeof(ProductPage));
            Routing.RegisterRoute(nameof(ProductDetailPage), typeof(ProductDetailPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(ChangePasswordPage), typeof(ChangePasswordPage));
            Routing.RegisterRoute(nameof(OrdersPage), typeof(OrdersPage));

            // Loaded += OnLoaded;
        }

        private async void OnLoaded(object? sender, EventArgs e)
        {
            Loaded -= OnLoaded;

            var hasSession = await _tokenService.HasStoredSessionAsync();

            if (!hasSession)
                await GoToAsync($"/{nameof(LoginPage)}");
        }

        private void OnSessionExpired(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await GoToAsync($"/{nameof(LoginPage)}");
            });
        }
    }
}
