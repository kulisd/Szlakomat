using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Domain.CommercialOffer;

/// <summary>
/// CatalogEntry - pozycja komercyjnej oferty.
/// Reprezentuje to, co organizacja aktualnie oferuje klientom.
///
/// Product (ProductType lub PackageType) mówi czym coś JEST (definicja biznesowa/operacyjna).
/// CatalogEntry mówi że coś JEST NA SPRZEDAŻ (dostępność komercyjna).
///
/// Kluczowe różnice od Product:
/// - displayName: nazwa marketingowa (vs nazwa techniczna)
/// - description: tekst sprzedażowy (vs opis techniczny)
/// - categories: do nawigacji/wyszukiwania
/// - validity: kiedy jest dostępne do zakupu
/// - metadata: elastyczne atrybuty (wyróżnione, znaczki, promocje itp.)
/// </summary>
internal class CatalogEntry
{
    private readonly CatalogEntryId _id;
    private readonly string _displayName;
    private readonly string _description;
    private readonly IProduct _product;
    private readonly IReadOnlySet<string> _categories;
    private readonly Validity _validity;
    private readonly IReadOnlyDictionary<string, string> _metadata;

    private CatalogEntry(
        CatalogEntryId? id,
        string? displayName,
        string? description,
        IProduct? product,
        IReadOnlySet<string>? categories,
        Validity? validity,
        IReadOnlyDictionary<string, string>? metadata)
    {
        Guard.IsNotNull(id);
        Guard.IsNotNullOrWhiteSpace(displayName);
        Guard.IsNotNullOrWhiteSpace(description);
        Guard.IsNotNull(product);
        Guard.IsNotNull(validity);

        _id = id;
        _displayName = displayName;
        _description = description;
        _product = product;
        _categories = categories ?? new HashSet<string>();
        _validity = validity;
        _metadata = metadata ?? new Dictionary<string, string>();
    }

    public static Builder CreateBuilder() => new();

    public CatalogEntryId Id() => _id;

    public string DisplayName() => _displayName;

    public string Description() => _description;

    public IProduct Product() => _product;

    public IReadOnlySet<string> Categories() => _categories;

    public Validity Validity() => _validity;

    public IReadOnlyDictionary<string, string> Metadata() => _metadata;

    /// <summary>
    /// Sprawdza czy pozycja jest dostępna do zakupu w danym dniu.
    /// </summary>
    public bool IsAvailableAt(DateOnly date) => _validity.IsValidAt(date);

    /// <summary>
    /// Sprawdza czy pozycja należy do danej kategorii.
    /// </summary>
    public bool IsInCategory(string category) => _categories.Contains(category);

    /// <summary>
    /// Returns metadata value for given key.
    /// </summary>
    public string? GetMetadata(string key) =>
        _metadata.TryGetValue(key, out var value) ? value : null;

    /// <summary>
    /// Returns metadata value or default if not present.
    /// </summary>
    public string GetMetadataOrDefault(string key, string defaultValue) =>
        _metadata.TryGetValue(key, out var value) ? value : defaultValue;

    /// <summary>
    /// Checks if metadata key exists.
    /// </summary>
    public bool HasMetadata(string key) => _metadata.ContainsKey(key);

    /// <summary>
    /// Creates a copy with updated validity (for discontinuation).
    /// </summary>
    public CatalogEntry WithValidity(Validity newValidity) => new(
        _id,
        _displayName,
        _description,
        _product,
        _categories,
        newValidity,
        _metadata
    );

    /// <summary>
    /// Creates a copy with updated metadata.
    /// </summary>
    public CatalogEntry WithMetadata(IReadOnlyDictionary<string, string> newMetadata) => new(
        _id,
        _displayName,
        _description,
        _product,
        _categories,
        _validity,
        newMetadata
    );

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj == null || GetType() != obj.GetType()) return false;
        CatalogEntry that = (CatalogEntry)obj;
        return _id.Equals(that._id);
    }

    public override int GetHashCode() => _id.GetHashCode();

    public override string ToString() =>
        $"CatalogEntry{{id={_id}, displayName='{_displayName}', product={_product}, categories={string.Join(",", _categories)}, validity={_validity}}}";

    public class Builder
    {
        private CatalogEntryId? _id;
        private string? _displayName;
        private string? _description;
        private IProduct? _product;
        private HashSet<string> _categories = new();
        private Validity? _validity;
        private Dictionary<string, string> _metadata = new();

        public Builder WithId(CatalogEntryId id)
        {
            _id = id;
            return this;
        }

        public Builder WithDisplayName(string displayName)
        {
            _displayName = displayName;
            return this;
        }

        public Builder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public Builder WithProduct(IProduct product)
        {
            _product = product;
            return this;
        }

        public Builder WithCategories(IEnumerable<string> categories)
        {
            _categories = new HashSet<string>(categories);
            return this;
        }

        public Builder WithCategory(string category)
        {
            _categories.Add(category);
            return this;
        }

        public Builder WithValidity(Validity validity)
        {
            _validity = validity;
            return this;
        }

        public Builder WithMetadata(Dictionary<string, string> metadata)
        {
            _metadata = new Dictionary<string, string>(metadata);
            return this;
        }

        public Builder WithMetadata(string key, string value)
        {
            _metadata[key] = value;
            return this;
        }

        public CatalogEntry Build()
        {
            return new CatalogEntry(
                _id!,
                _displayName!,
                _description!,
                _product!,
                _categories,
                _validity!,
                _metadata
            );
        }
    }
}
