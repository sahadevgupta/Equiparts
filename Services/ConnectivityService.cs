using Equiparts.Interfaces;
using Microsoft.Extensions.Logging;

namespace Equiparts.Services;

public sealed class ConnectivityService : IConnectivityService, IDisposable
{
    // A tiny, unauthenticated endpoint used only to prove a real route to the internet
    // exists - the same technique OS connectivity managers use for captive-portal checks.
    private const string ReachabilityProbeUrl = "https://www.gstatic.com/generate_204";

    private static readonly TimeSpan ProbeTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(20);

    private readonly IConnectivity _connectivity;
    private readonly ILogger<ConnectivityService> _logger;
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _checkLock = new(1, 1);
    private readonly Timer _pollTimer;

    private volatile bool _isConnected = true;

    public event EventHandler<bool>? ConnectivityChanged;

    public bool IsConnected => _isConnected;

    public ConnectivityService(IConnectivity connectivity, ILogger<ConnectivityService> logger)
    {
        _connectivity = connectivity;
        _logger = logger;
        _httpClient = new HttpClient { Timeout = ProbeTimeout };

        _connectivity.ConnectivityChanged += OnOsConnectivityChanged;

        // Also poll on a timer: a WiFi network that never changes OS-reported state but
        // has no real internet route (or drops it later) would otherwise never be caught.
        _pollTimer = new Timer(_ => _ = CheckInternetAccessAsync(), null, TimeSpan.Zero, PollingInterval);
    }

    private void OnOsConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        _ = CheckInternetAccessAsync();
    }

    public async Task<bool> CheckInternetAccessAsync(CancellationToken cancellationToken = default)
    {
        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            SetConnected(false);
            return false;
        }

        await _checkLock.WaitAsync(cancellationToken);
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, ReachabilityProbeUrl);
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            var reachable = response.IsSuccessStatusCode || (int)response.StatusCode == 204;
            SetConnected(reachable);
            return reachable;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or TimeoutException)
        {
            _logger.LogWarning("Internet reachability probe failed; treating device as offline.");
            SetConnected(false);
            return false;
        }
        finally
        {
            _checkLock.Release();
        }
    }

    private void SetConnected(bool isConnected)
    {
        if (_isConnected == isConnected)
            return;

        _isConnected = isConnected;
        _logger.LogInformation(isConnected ? "Internet connection restored." : "Internet connection lost.");
        ConnectivityChanged?.Invoke(this, isConnected);
    }

    public void Dispose()
    {
        _connectivity.ConnectivityChanged -= OnOsConnectivityChanged;
        _pollTimer.Dispose();
        _httpClient.Dispose();
        _checkLock.Dispose();
    }
}
