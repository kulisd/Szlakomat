namespace Szlakomat.Products.Domain.Catalog.Features;

/// <summary>
/// Container for all feature instances of a ProductInstance.
/// Provides convenient access to feature values by name.
/// </summary>
internal class ProductFeatureInstances
{
    private readonly IReadOnlyDictionary<string, ProductFeatureInstance> _features;

    public ProductFeatureInstances(IEnumerable<ProductFeatureInstance>? instances)
    {
        Guard.IsNotNull(instances);

        _features = instances.ToDictionary(
            inst => inst.FeatureType.Name,
            inst => inst
        );
    }

    public static ProductFeatureInstances Empty() => new(Enumerable.Empty<ProductFeatureInstance>());

    public static ProductFeatureInstances Of(params ProductFeatureInstance[] instances) => new(instances);

    public ProductFeatureInstance? Get(string featureName) =>
        _features.TryGetValue(featureName, out var inst) ? inst : null;

    public ProductFeatureInstance? Get(ProductFeatureType featureType) =>
        _features.Values.FirstOrDefault(it => it.IsOfType(featureType));

    public bool Has(string featureName) => _features.ContainsKey(featureName);

    public bool Has(ProductFeatureType featureType) => Has(featureType.Name);

    public IEnumerable<ProductFeatureInstance> All() => _features.Values;

    public int Size => _features.Count;

    public bool IsEmpty => _features.Count == 0;

    public void ValidateAgainst(ProductFeatureTypes featureTypes)
    {
        var mandatoryFeatures = featureTypes.MandatoryFeatures();

        foreach (var mandatory in mandatoryFeatures)
        {
            if (!Has(mandatory.Name))
                throw new ArgumentException($"Mandatory feature '{mandatory.Name}' is missing");
        }

        foreach (var featureName in _features.Keys)
        {
            if (!featureTypes.Has(featureName))
                throw new ArgumentException($"Feature '{featureName}' is not defined in ProductType");
        }
    }

    public override string ToString() =>
        $"ProductFeatureInstances{{{string.Join(", ", _features.Values.Select(f => $"{f.FeatureType.Name}={f.Value}"))}}}";
}
