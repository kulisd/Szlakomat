using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;
using Szlakomat.Products.Domain.Catalog.Features;

namespace Szlakomat.Products.Domain.Catalog.ProductType;

/// <summary>
/// ProductType reprezentuje typ/definicję produktu (nie konkretny egzemplarz).
/// Przykłady: "iPhone 15 Pro 256GB", "Czysty kod (książka)", "Mleko organiczne 1L"
///
/// Każdy ProductType jest unikalnie identyfikowany przez ProductIdentifier (ISBN, GTIN, UUID itp.)
/// i definiuje sposób śledzenia oraz mierzenia egzemplarzy.
///
/// ProductType może definiować obowiązkowe i opcjonalne typy cech, które egzemplarze muszą lub mogą posiadać.
///
/// To jest liść we wzorcu kompozytowym - zwykły produkt (nie pakiet).
/// </summary>
internal class ProductType : IProduct
{
    private readonly IProductIdentifier _id;
    private readonly ProductName _name;
    private readonly ProductDescription _description;
    private readonly Unit _preferredUnit;
    private readonly ProductTrackingStrategy _trackingStrategy;
    private readonly ProductFeatureTypes _featureTypes;
    private readonly ProductMetadata _metadata;
    private readonly IApplicabilityConstraint _applicabilityConstraint;

    internal ProductType(
        IProductIdentifier? id,
        ProductName? name,
        ProductDescription? description,
        Unit? preferredUnit,
        ProductTrackingStrategy trackingStrategy,
        ProductFeatureTypes? featureTypes,
        ProductMetadata? metadata,
        IApplicabilityConstraint? applicabilityConstraint)
    {
        Guard.IsNotNull(id);
        Guard.IsNotNull(name);
        Guard.IsNotNull(description);
        Guard.IsNotNull(preferredUnit);
        Guard.IsNotNull(trackingStrategy);
        Guard.IsNotNull(featureTypes);
        Guard.IsNotNull(metadata);
        Guard.IsNotNull(applicabilityConstraint);

        _id = id;
        _name = name;
        _description = description;
        _preferredUnit = preferredUnit;
        _trackingStrategy = trackingStrategy;
        _featureTypes = featureTypes;
        _metadata = metadata;
        _applicabilityConstraint = applicabilityConstraint;
    }

    /// <summary>
    /// Tworzy prosty typ produktu do celów testowania.
    /// </summary>
    public static ProductType Define(
        IProductIdentifier id,
        ProductName name,
        ProductDescription description) =>
        new(
            id, name, description,
            Unit.Pieces(),
            ProductTrackingStrategy.Identical,
            ProductFeatureTypes.Empty(),
            ProductMetadata.Empty(),
            Szlakomat.Products.Domain.Common.Applicability.ApplicabilityConstraint.AlwaysTrue());

    /// <summary>
    /// Tworzy unikalny typ produktu (jeden egzemplarz).
    /// Jednostka jest domyślnie sztuki, ponieważ zawsze jest dokładnie 1 egzemplarz.
    /// </summary>
    public static ProductType Unique(
        IProductIdentifier id,
        ProductName name,
        ProductDescription description) =>
        new(
            id, name, description,
            Unit.Pieces(),
            ProductTrackingStrategy.Unique,
            ProductFeatureTypes.Empty(),
            ProductMetadata.Empty(),
            Szlakomat.Products.Domain.Common.Applicability.ApplicabilityConstraint.AlwaysTrue());

    /// <summary>
    /// Tworzy typ produktu, gdzie każdy egzemplarz jest śledzony indywidualnie (np. numer seryjny).
    /// </summary>
    public static ProductType IndividuallyTracked(
        IProductIdentifier id,
        ProductName name,
        ProductDescription description,
        Unit preferredUnit) =>
        new(
            id, name, description,
            preferredUnit,
            ProductTrackingStrategy.IndividuallyTracked,
            ProductFeatureTypes.Empty(),
            ProductMetadata.Empty(),
            Szlakomat.Products.Domain.Common.Applicability.ApplicabilityConstraint.AlwaysTrue());

    /// <summary>
    /// Tworzy typ produktu, gdzie egzemplarze są śledzone przez partię produkcyjną.
    /// </summary>
    public static ProductType BatchTracked(
        IProductIdentifier id,
        ProductName name,
        ProductDescription description,
        Unit preferredUnit) =>
        new(
            id, name, description,
            preferredUnit,
            ProductTrackingStrategy.BatchTracked,
            ProductFeatureTypes.Empty(),
            ProductMetadata.Empty(),
            Szlakomat.Products.Domain.Common.Applicability.ApplicabilityConstraint.AlwaysTrue());

    /// <summary>
    /// Tworzy typ produktu, gdzie egzemplarze są śledzone zarówno indywidualnie, jak i przez partię.
    /// </summary>
    public static ProductType IndividuallyAndBatchTracked(
        IProductIdentifier id,
        ProductName name,
        ProductDescription description,
        Unit preferredUnit) =>
        new(
            id, name, description,
            preferredUnit,
            ProductTrackingStrategy.IndividuallyAndBatchTracked,
            ProductFeatureTypes.Empty(),
            ProductMetadata.Empty(),
            Szlakomat.Products.Domain.Common.Applicability.ApplicabilityConstraint.AlwaysTrue());

    /// <summary>
    /// Tworzy typ produktu, gdzie egzemplarze są zamienne (identyczne).
    /// </summary>
    public static ProductType Identical(
        IProductIdentifier id,
        ProductName name,
        ProductDescription description,
        Unit preferredUnit) =>
        new(
            id, name, description,
            preferredUnit,
            ProductTrackingStrategy.Identical,
            ProductFeatureTypes.Empty(),
            ProductMetadata.Empty(),
            Szlakomat.Products.Domain.Common.Applicability.ApplicabilityConstraint.AlwaysTrue());

    /// <summary>
    /// Zwraca budowniczego dla ProductType z określonymi parametrami śledzenia i pomiaru.
    /// </summary>
    public static ProductBuilder.ProductTypeBuilder Builder(
        IProductIdentifier id,
        ProductName name,
        ProductDescription description,
        Unit preferredUnit,
        ProductTrackingStrategy trackingStrategy) =>
        new ProductBuilder(id, name, description).AsProductType(preferredUnit, trackingStrategy);

    public IProductIdentifier Id() => _id;

    public ProductName Name() => _name;

    public ProductDescription Description() => _description;

    public Unit PreferredUnit() => _preferredUnit;

    public ProductTrackingStrategy TrackingStrategy() => _trackingStrategy;

    public ProductFeatureTypes FeatureTypes() => _featureTypes;

    public ProductMetadata Metadata() => _metadata;

    public IApplicabilityConstraint ApplicabilityConstraint() => _applicabilityConstraint;

    public IProductIdentifier Identifier() => _id;

    public bool IsApplicableFor(ApplicabilityContext context) =>
        _applicabilityConstraint.IsSatisfiedBy(context);

    public override string ToString() =>
        $"ProductType{{id={_id}, name={_name}, unit={_preferredUnit}, tracking={_trackingStrategy}, features={_featureTypes}}}";
}
