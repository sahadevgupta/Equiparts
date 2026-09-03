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
