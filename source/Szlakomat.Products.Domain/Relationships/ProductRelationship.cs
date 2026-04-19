using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.Relationships;

public record ProductRelationship(
    ProductRelationshipId Id,
    IProductIdentifier From,
    IProductIdentifier To,
    ProductRelationshipType Type)
{
    public static ProductRelationship Of(ProductRelationshipId id, IProductIdentifier from, IProductIdentifier to,
        ProductRelationshipType type) =>
        new(id, from, to, type);
}
