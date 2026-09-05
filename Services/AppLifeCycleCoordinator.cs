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
        private readonly ILogger<AppLifeCycleCoordinator> _logger;

        public AppLifeCycleCoordinator(ITokenService tokenService,
            ILogger<AppLifeCycleCoordinator> logger,
            INavigationService navigationService)
        {
            _navigationService = navigationService;
            _tokenService = tokenService;
            _logger = logger;
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
            await CheckUserLoggedInStateAsync();
        }

        private async Task CheckUserLoggedInStateAsync()
        {
            var hasSession = await _tokenService.HasStoredSessionAsync();
            if (hasSession)
            {
                await Shell.Current.GoToAsync("//app/home");
            }
            else
            {
                await _navigationService.NaviagteAsync<LoginPage>(true);
            }
        }
    }
}