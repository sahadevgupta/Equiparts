using Equiparts.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Equiparts.Controls;

// Drop this into any page's XAML to get a reactive "no internet" banner backed by the
// single app-wide IConnectivityService. Resolves via IPlatformApplication.Current.Services
// since XAML-instantiated ContentViews don't go through constructor injection.
public partial class OfflineBannerView : ContentView
{
    private readonly IConnectivityService? _connectivityService;

    public OfflineBannerView()
    {
        InitializeComponent();

        _connectivityService = IPlatformApplication.Current?.Services.GetService<IConnectivityService>();

        if (_connectivityService is null)
            return;

        IsVisible = !_connectivityService.IsConnected;
        _connectivityService.ConnectivityChanged += OnConnectivityChanged;
        Unloaded += OnUnloaded;
    }

    private void OnConnectivityChanged(object? sender, bool isConnected)
    {
        MainThread.BeginInvokeOnMainThread(() => IsVisible = !isConnected);
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        Unloaded -= OnUnloaded;

        if (_connectivityService is not null)
            _connectivityService.ConnectivityChanged -= OnConnectivityChanged;
    }
}
