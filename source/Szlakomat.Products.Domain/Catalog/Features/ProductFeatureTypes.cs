namespace Szlakomat.Products.Domain.Catalog.Features;

/// <summary>
/// Container for all feature type definitions of a ProductType.
/// Provides convenient access to features by name and filtering by mandatory/optional status.
/// </summary>
internal class ProductFeatureTypes
{
    private readonly IReadOnlyDictionary<string, ProductFeatureTypeDefinition> _features;

    public ProductFeatureTypes(IEnumerable<ProductFeatureTypeDefinition>? definitions)
    {
        Guard.IsNotNull(definitions);

        _features = definitions.ToDictionary(
            def => def.FeatureType.Name,
            def => def
        );
    }

    public static ProductFeatureTypes Empty() => new(Enumerable.Empty<ProductFeatureTypeDefinition>());

    public static ProductFeatureTypes Of(params ProductFeatureTypeDefinition[] definitions) => new(definitions);

    public ProductFeatureTypeDefinition? Get(string featureName) =>
        _features.TryGetValue(featureName, out var def) ? def : null;

    public ProductFeatureType? GetFeatureType(string featureName) =>
        Get(featureName)?.FeatureType;

    public bool Has(string featureName) => _features.ContainsKey(featureName);

    public bool IsMandatory(string featureName) =>
        Get(featureName)?.Mandatory ?? false;

    public IReadOnlySet<ProductFeatureType> MandatoryFeatures() =>
        _features.Values
            .Where(d => d.Mandatory)
            .Select(d => d.FeatureType)
            .ToHashSet();

    public IReadOnlySet<ProductFeatureType> OptionalFeatures() =>
        _features.Values
            .Where(d => !d.Mandatory)
            .Select(d => d.FeatureType)
            .ToHashSet();

    public IReadOnlySet<ProductFeatureType> AllFeatures() =>
        _features.Values
            .Select(d => d.FeatureType)
            .ToHashSet();

    public int Size => _features.Count;

    public bool IsEmpty => _features.Count == 0;

    public override string ToString() =>
        $"ProductFeatureTypes{{mandatory={MandatoryFeatures().Count}, optional={OptionalFeatures().Count}}}";
}
