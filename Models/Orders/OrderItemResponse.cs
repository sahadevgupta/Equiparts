using System.Text.Json.Serialization;

namespace Equiparts.Models.Orders;

public class OrderItemResponse
{
    [JsonPropertyName("partNumber")]
    public string? PartNumber { get; set; }

    [JsonPropertyName("productName")]
    public string? ProductName { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public double UnitPrice { get; set; }

    [JsonPropertyName("taxAmount")]
    public double TaxAmount { get; set; }

    [JsonPropertyName("lineTotal")]
    public double LineTotal { get; set; }
}
