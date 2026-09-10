using CommunityToolkit.Maui.Core;
using Equiparts.Enums;

namespace Equiparts.Interfaces;

public interface IDialogService
{
    Task ShowAlertAsync(string message, AlertType alertType = AlertType.Warning);

    Task<bool> ShowConfirmAsync(string message, string acceptText = "Yes", string cancelText = "Cancel", AlertType alertType = AlertType.Warning);

    void ShowToast(string message, ToastDuration duration = ToastDuration.Short);
}
