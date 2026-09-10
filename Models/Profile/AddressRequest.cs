using System.Text.Json.Serialization;

namespace Equiparts.Models.Profile;

public class AddressRequest
{
    // [JsonPropertyName("label")]
    // public string? Label { get; set; }

    [JsonPropertyName("contactName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("line1")]
    public string AddressLine1 { get; set; } = string.Empty;

    [JsonPropertyName("line2")]
    public string? AddressLine2 { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("contactPhone")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("landmark")]
    public string? Landmark { get; set; }

    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; set; }
}
