namespace Equiparts.Models;

public class SimilarProduct
{
    public Product Product { get; set; } = new();

    // "samePart", "sameCategory" or "sameType" as sent by the backend.
    public string? MatchType { get; set; }

    public string? MatchReason { get; set; }

    public string MatchLabel => MatchType switch
    {
        "samePart" => "Same Part",
        "sameCategory" => "Same Category",
        "sameType" => "Same Type",
        _ => string.Empty
    };

    public bool HasMatchLabel => !string.IsNullOrEmpty(MatchLabel);

    // Same part number from another brand/tier is the strongest match, so it gets the accent badge.
    public bool IsSamePart => MatchType == "samePart";
}
