using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equiparts.Interfaces;
using Equiparts.Views;
using Microsoft.Extensions.Logging;

namespace Equiparts.Services
{
    public class AppLifeCycleCoordinator : IAppLifeCycleCoordinator
    {
        private readonly INavigationService _navigationService;
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AppLifeCycleCoordinator> _logger;

        // Several in-flight requests can each observe the dead session; only the first
        // one should navigate to Login.
        private int _isHandlingSessionExpiry;

        public AppLifeCycleCoordinator(ITokenService tokenService,
            ICurrentUserService currentUserService,
            ILogger<AppLifeCycleCoordinator> logger,
            INavigationService navigationService)
        {
            _navigationService = navigationService;
            _tokenService = tokenService;
            _currentUserService = currentUserService;
            _logger = logger;

            // Raised only when the server genuinely rejects the refresh token (never for an
            // expired access token or a network failure), so the stored session is already
            // gone - send the user to Login now instead of leaving them on a page whose
            // authenticated calls silently fail until the next launch.
            _tokenService.SessionExpired += OnSessionExpired;
        }
        public void RegisterGlobalExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                try
                {
                    var exception = e.ExceptionObject as Exception;
                    _logger.LogError(exception, "Unhandled exception caught : {Message}", exception?.Message);
                }
                catch
                {

                }
            };

            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                try
                {
                    _logger.LogError(e.Exception, "Unobserved task exception caught : {Message}", e.Exception?.Message);
                    e.SetObserved();
                }
                catch
                {

                }
            };
        }

        public async Task OnWindowCreatedAsync()
        {
            // Show the splash screen first; it calls NavigateToInitialDestinationAsync()
            // itself once its entrance animation has played out.
            await Shell.Current.GoToAsync("//loading");
        }

        public async Task NavigateToInitialDestinationAsync()
        {
            var hasSession = await _tokenService.HasStoredSessionAsync();
            _logger.LogInformation("App launch: stored session found: {HasSession}.", hasSession);

            if (hasSession)
            {
                await Shell.Current.GoToAsync("//app/home");
            }
            else
            {
                await _navigationService.NaviagteAsync<LoginPage>(true);
            }
        }

        private void OnSessionExpired(object? sender, EventArgs e)
        {
            if (Interlocked.Exchange(ref _isHandlingSessionExpiry, 1) == 1)
                return;

            _logger.LogWarning("Session expired; redirecting to Login.");
            _currentUserService.Clear();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    if (Shell.Current?.CurrentPage is not LoginPage)
                        await _navigationService.NaviagteAsync<LoginPage>(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to navigate to Login after session expiry.");
                }
                finally
                {
                    Interlocked.Exchange(ref _isHandlingSessionExpiry, 0);
                }
            });
        }
    }
}