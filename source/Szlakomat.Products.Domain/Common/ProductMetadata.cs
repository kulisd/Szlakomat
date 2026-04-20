namespace Szlakomat.Products.Domain.Common;

/// <summary>
/// ProductMetadata represents static, unchanging properties of a ProductType.
/// These are simple key-value pairs that describe fixed characteristics of the product type.
///
/// Examples:
/// - category: "coffee", "electronics", "clothing"
/// - seasonal: "true", "false"
/// - season: "winter", "summer"
/// - brand: "Apple", "Samsung"
/// - material: "cotton", "polyester"
/// </summary>
public class ProductMetadata
{
    private readonly IReadOnlyDictionary<string, string> _data;

    private ProductMetadata(IReadOnlyDictionary<string, string> data)
    {
        _data = new Dictionary<string, string>(data);
    }

    public static ProductMetadata Empty() => new(new Dictionary<string, string>());

    public static ProductMetadata Of(Dictionary<string, string>? data) => new(data ?? new Dictionary<string, string>());

    public string? Get(string key) => _data.TryGetValue(key, out var value) ? value : null;

    public string GetOrDefault(string key, string defaultValue) =>
        Get(key) ?? defaultValue;

    public bool Has(string key) => _data.ContainsKey(key);

    public IReadOnlyDictionary<string, string> AsMap() => _data;

    public ProductMetadata With(string key, string value)
    {
        var newData = new Dictionary<string, string>(_data) { [key] = value };
        return new ProductMetadata(newData);
    }

    public override string ToString() => $"ProductMetadata{_data}";
}
