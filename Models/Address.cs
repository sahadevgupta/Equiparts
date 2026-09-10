namespace Equiparts.Models;

public class Address
{
    public int Id { get; set; }

    public string? Label { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Line1 { get; set; } = string.Empty;

    public string? Line2 { get; set; }

    public string? Type { get; set; }

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? Landmark { get; set; }

    public bool IsDefault { get; set; }

    public string DisplayLines =>
        string.Join(", ", new[] { Line1, Line2, Landmark, City, State }
            .Where(part => !string.IsNullOrWhiteSpace(part)));
}
