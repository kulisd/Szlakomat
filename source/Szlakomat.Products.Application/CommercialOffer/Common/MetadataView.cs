namespace Szlakomat.Products.Application.CommercialOffer.Common;

/// <summary>
/// Metadata for a catalog entry or product type.
/// Generic key-value view for flexible attributes.
/// </summary>
public record MetadataView(
    IReadOnlyDictionary<string, string> Attributes
)
{
    public string? Get(string key) =>
        Attributes.TryGetValue(key, out var value) ? value : null;

    public string GetOrDefault(string key, string defaultValue) =>
        Attributes.TryGetValue(key, out var value) ? value : defaultValue;

    public bool Has(string key) =>
        Attributes.ContainsKey(key);
}
