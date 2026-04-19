using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Catalog.ProductType;

namespace Szlakomat.Products.Domain.Catalog.PackageType;

/// <summary>
/// PackageType reprezentuje produkt złożony z innych produktów.
///
/// Przykłady:
/// - Pakiet laptopa (laptop + torba + mysz + ubezpieczenie)
/// - Pakiet bankowy (konto + karta + ubezpieczenie)
/// - Ustawienie biurowe (pakiet sprzętu + pakiet oprogramowania + wsparcie)
///
/// PackageType może zawierać inne PackageTypes (zagnieżdżone pakiety), tworząc strukturę kompozytową.
/// To jest kompozyt we wzorcu kompozytowym.
/// </summary>
internal class PackageType : IProduct
{
    private readonly IProductIdentifier _id;
    private readonly ProductName _name;
    private readonly ProductDescription _description;
    private readonly ProductTrackingStrategy _trackingStrategy;
    private readonly ProductMetadata _metadata;
    private readonly IApplicabilityConstraint _applicabilityConstraint;
    private readonly PackageStructure _structure;

    internal PackageType(
        IProductIdentifier? id,
        ProductName? name,
        ProductDescription? description,
        ProductTrackingStrategy trackingStrategy,
        ProductMetadata? metadata,
        IApplicabilityConstraint? applicabilityConstraint,
        PackageStructure? structure)
    {
        Guard.IsNotNull(id);
        Guard.IsNotNull(name);
        Guard.IsNotNull(description);
        Guard.IsNotNull(trackingStrategy);
        Guard.IsNotNull(metadata);
        Guard.IsNotNull(applicabilityConstraint);
        Guard.IsNotNull(structure);

        _id = id;
        _name = name;
        _description = description;
        _trackingStrategy = trackingStrategy;
        _metadata = metadata;
        _applicabilityConstraint = applicabilityConstraint;
        _structure = structure;
    }

    /// <summary>
    /// Tworzy pakiet z ustawieniami domyślnymi (śledzenie ŚLEDZONY_INDYWIDUALNIE, bez ograniczeń stosowalności).
    /// </summary>
    public static PackageType Define(
        IProductIdentifier id,
        ProductName name,
        ProductDescription description,
        PackageStructure structure) =>
        new(
            id, name, description,
            ProductTrackingStrategy.IndividuallyTracked,
            ProductMetadata.Empty(),
            Szlakomat.Products.Domain.Common.Applicability.ApplicabilityConstraint.AlwaysTrue(),
            structure);

    public IProductIdentifier Id() => _id;

    public ProductName Name() => _name;

    public ProductDescription Description() => _description;

    public ProductMetadata Metadata() => _metadata;

    public IApplicabilityConstraint ApplicabilityConstraint() => _applicabilityConstraint;

    public ProductTrackingStrategy TrackingStrategy() => _trackingStrategy;

    public PackageStructure Structure() => _structure;

    /// <summary>
    /// Waliduje czy wybrane produkty odpowiadają regułom struktury pakietu.
    /// </summary>
    public PackageValidationResult ValidateSelection(List<SelectedProduct> selection) =>
        _structure.Validate(selection);

    public override string ToString() =>
        $"PackageType{{id={_id}, name={_name}, tracking={_trackingStrategy}, structure={_structure}}}";
}
