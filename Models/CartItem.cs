using CommunityToolkit.Mvvm.ComponentModel;

namespace Equiparts.Models;

public partial class CartItem : ObservableObject
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public string? PartNumber { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    [ObservableProperty]
    private int quantity;

    public decimal UnitPrice { get; set; }

    public double GstRatePercent { get; set; }

    [ObservableProperty]
    private decimal lineSubTotal;

    [ObservableProperty]
    private decimal lineTax;

    [ObservableProperty]
    private decimal lineTotal;

    /// <summary>True while an Increase/Decrease/Remove request for this item is in flight.</summary>
    [ObservableProperty]
    private bool isUpdating;

    public void CopyMutableFieldsFrom(CartItem other)
    {
        Quantity = other.Quantity;
        LineSubTotal = other.LineSubTotal;
        LineTax = other.LineTax;
        LineTotal = other.LineTotal;
    }
}
