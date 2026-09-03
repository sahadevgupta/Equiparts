namespace Equiparts.Interfaces;

// Centralized connectivity check. Deliberately more than Connectivity.NetworkAccess:
// a device can report NetworkAccess.Internet while actually sitting behind a captive
// portal or a WiFi with no real route out, so this backs the OS signal with a real
// reachability probe and polls periodically to catch that case too.
public interface IConnectivityService
{
    bool IsConnected { get; }

    event EventHandler<bool>? ConnectivityChanged;

    Task<bool> CheckInternetAccessAsync(CancellationToken cancellationToken = default);
}
