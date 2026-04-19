namespace Szlakomat.Products.Domain.CommercialOffer;

/// <summary>
/// Unique identifier for a CatalogEntry (may differ from ProductIdentifier).
///
/// The same ProductType can appear in multiple catalog entries:
/// - Different marketing campaigns
/// - Different markets/regions
/// - Different time periods
/// </summary>
public class CatalogEntryId
{
    public string Value { get; }

    private CatalogEntryId(string? value)
    {
        Guard.IsNotNullOrWhiteSpace(value);
        Value = value;
    }

    public static CatalogEntryId Of(string value) => new(value);

    public static CatalogEntryId Generate() => new($"CATALOG-{Guid.NewGuid()}");

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj == null || GetType() != obj.GetType()) return false;
        CatalogEntryId that = (CatalogEntryId)obj;
        return Value == that.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
