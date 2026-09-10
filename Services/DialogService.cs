using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Equiparts.Controls;
using Equiparts.Enums;
using Equiparts.Extensions;
using Equiparts.Interfaces;
using Mopups.Interfaces;

namespace Equiparts.Services;

public class DialogService(IPopupNavigation popupNavigation) : IDialogService
{
    public async Task ShowAlertAsync(string message, AlertType alertType = AlertType.Warning)
    {
        await ShowAsync(message, alertType, "OK", null);
    }

    public async Task<bool> ShowConfirmAsync(string message, string acceptText = "Yes", string cancelText = "Cancel", AlertType alertType = AlertType.Warning)
    {
        return await ShowAsync(message, alertType, acceptText, cancelText);
    }

    public void ShowToast(string message, ToastDuration duration = ToastDuration.Short)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var toast = Toast.Make(message, duration);
            await toast.Show();
        });
    }

    private async Task<bool> ShowAsync(string message, AlertType alertType, string acceptText, string? cancelText)
    {
        var (icon, tintColor) = GetIconAndColor(alertType);

        var popup = new CustomAlertPopup
        {
            Message = message,
            Icon = icon,
            IconTintColor = tintColor,
            AcceptText = acceptText,
            CancelText = cancelText
        };

        return await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await popupNavigation.PushAsync(popup);
            return await popup.Result;
        });
    }

    private static (string Icon, Color TintColor) GetIconAndColor(AlertType alertType)
    {
        var resources = Application.Current?.Resources;

        return alertType switch
        {
            AlertType.Success => (FontAwesomeIcons.CheckCircle, GetColor(resources, "Success", Colors.Green)),
            AlertType.Error => (FontAwesomeIcons.TimesCircle, GetColor(resources, "Danger", Colors.Red)),
            _ => (FontAwesomeIcons.ExclamationTriangle, GetColor(resources, "Warning", Colors.Orange)),
        };
    }

    private static Color GetColor(ResourceDictionary? resources, string key, Color fallback) =>
        resources is not null && resources.TryGetValue(key, out var value) && value is Color color ? color : fallback;
}
