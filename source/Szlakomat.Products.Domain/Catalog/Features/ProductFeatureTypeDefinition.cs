namespace Szlakomat.Products.Domain.Catalog.Features;

/// <summary>
/// Defines a product feature type along with whether it's mandatory or optional.
/// </summary>
internal class ProductFeatureTypeDefinition
{
    public ProductFeatureType FeatureType { get; }
    public bool Mandatory { get; }

    public ProductFeatureTypeDefinition(ProductFeatureType? featureType, bool mandatory)
    {
        Guard.IsNotNull(featureType);
        FeatureType = featureType;
        Mandatory = mandatory;
    }

    public static ProductFeatureTypeDefinition MakeMandatory(ProductFeatureType featureType) => new(featureType, true);

    public static ProductFeatureTypeDefinition MakeOptional(ProductFeatureType featureType) => new(featureType, false);

    public bool IsOptional => !Mandatory;

    public override string ToString() => $"{(Mandatory ? "mandatory" : "optional")}({FeatureType.Name})";
}
