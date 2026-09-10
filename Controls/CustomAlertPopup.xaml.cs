using Mopups.Pages;
using Mopups.Services;

namespace Equiparts.Controls;

public partial class CustomAlertPopup : PopupPage
{
    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(CustomAlertPopup), string.Empty);

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(CustomAlertPopup), string.Empty);

    public static readonly BindableProperty IconTintColorProperty =
        BindableProperty.Create(nameof(IconTintColor), typeof(Color), typeof(CustomAlertPopup), default(Color));

    public static readonly BindableProperty AcceptTextProperty =
        BindableProperty.Create(nameof(AcceptText), typeof(string), typeof(CustomAlertPopup), "OK");

    public static readonly BindableProperty CancelTextProperty = BindableProperty.Create(
        nameof(CancelText),
        typeof(string),
        typeof(CustomAlertPopup),
        null,
        propertyChanged: OnCancelTextChanged);

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public Color IconTintColor
    {
        get => (Color)GetValue(IconTintColorProperty);
        set => SetValue(IconTintColorProperty, value);
    }

    public string AcceptText
    {
        get => (string)GetValue(AcceptTextProperty);
        set => SetValue(AcceptTextProperty, value);
    }

    public string? CancelText
    {
        get => (string?)GetValue(CancelTextProperty);
        set => SetValue(CancelTextProperty, value);
    }

    private readonly TaskCompletionSource<bool> _resultCompletionSource = new();

    public Task<bool> Result => _resultCompletionSource.Task;

    public CustomAlertPopup()
    {
        InitializeComponent();
    }

    static void OnCancelTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var popup = (CustomAlertPopup)bindable;
        popup.CancelButton.IsVisible = !string.IsNullOrWhiteSpace((string?)newValue);
    }

    async void OnAcceptClicked(object sender, EventArgs e) => await CloseWithResultAsync(true);

    async void OnCancelClicked(object sender, EventArgs e) => await CloseWithResultAsync(false);

    private async Task CloseWithResultAsync(bool result)
    {
        _resultCompletionSource.TrySetResult(result);
        await MopupService.Instance.PopAsync();
    }
}
