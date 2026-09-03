using Equiparts.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Equiparts
{
    public partial class App : Application
    {
        private readonly Lazy<IAppLifeCycleCoordinator> _coordinator;
        private readonly IServiceProvider _serviceProvider;

        public App(Lazy<IAppLifeCycleCoordinator> coordinator, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _coordinator = coordinator;
            _serviceProvider = serviceProvider;

            UserAppTheme = AppTheme.Light;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            window.Created += async (_, _) =>
            {
                await _coordinator.Value.OnWindowCreatedAsync();
            };
            return window;
        }
    }
}