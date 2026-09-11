using System.Collections.ObjectModel;
using Equiparts.Models;
using Mopups.Pages;
using Mopups.Services;

namespace Equiparts.Controls;

public sealed class AddressPickerItem
{
    public required Address Address { get; init; }

    public bool IsSelected { get; init; }
}

public sealed class AddressSelectorResult
{
    public Address? SelectedAddress { get; init; }

    public bool IsAddNew { get; init; }

    public Address? EditAddress { get; init; }
}

public partial class AddressSelectorPopup : PopupPage
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(AddressSelectorPopup), "Select Delivery Address");

    private readonly TaskCompletionSource<AddressSelectorResult?> _resultCompletionSource = new();

    public ObservableCollection<AddressPickerItem> Items { get; } = [];

    public Task<AddressSelectorResult?> Result => _resultCompletionSource.Task;

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public AddressSelectorPopup(IReadOnlyList<Address> addresses, int selectedAddressId, string? title = null)
    {
        InitializeComponent();
        BindingContext = this;

        if (title is not null)
            Title = title;

        foreach (var address in addresses)
            Items.Add(new AddressPickerItem { Address = address, IsSelected = address.Id == selectedAddressId });
    }

    async void OnAddressTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Address address)
            await CloseWithResultAsync(new AddressSelectorResult { SelectedAddress = address });
    }

    async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton { CommandParameter: Address address })
            await CloseWithResultAsync(new AddressSelectorResult { EditAddress = address });
    }

    async void OnAddNewClicked(object sender, TappedEventArgs e) =>
        await CloseWithResultAsync(new AddressSelectorResult { IsAddNew = true });

    async void OnCloseClicked(object sender, EventArgs e) => await CloseWithResultAsync(null);

    private async Task CloseWithResultAsync(AddressSelectorResult? result)
    {
        await MopupService.Instance.PopAsync();
        _resultCompletionSource.TrySetResult(result);
    }
}
