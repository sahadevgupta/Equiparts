using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Models;
using System.Collections.ObjectModel;

namespace Equiparts.ViewModels;

public partial class CartViewModel : BaseViewModel
{
    public ObservableCollection<CartItem> Items { get; } = [];

    public decimal GrandTotal => Items.Sum(x => x.Total);

    [RelayCommand]
    void Increase(CartItem item)
    {
        item.Quantity++;

        OnPropertyChanged(nameof(GrandTotal));
    }

    [RelayCommand]
    void Decrease(CartItem item)
    {
        if (item.Quantity > 1)
            item.Quantity--;

        OnPropertyChanged(nameof(GrandTotal));
    }

    [RelayCommand]
    void Remove(CartItem item)
    {
        Items.Remove(item);

        OnPropertyChanged(nameof(GrandTotal));
    }

    [RelayCommand]
    async Task Checkout()
    {
        // Navigate
    }
}