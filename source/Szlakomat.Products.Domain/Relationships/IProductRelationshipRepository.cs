using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.Relationships;

/// <summary>
/// Repository for ProductRelationship persistence.
/// </summary>
internal interface IProductRelationshipRepository
{
    List<ProductRelationship> FindAllRelationsFrom(IProductIdentifier productIdentifier);
    List<ProductRelationship> FindAllRelationsFrom(IProductIdentifier productIdentifier, ProductRelationshipType type);
    ProductRelationship? FindBy(ProductRelationshipId relationshipId);
    void Save(ProductRelationship productRelationship);
    ProductRelationshipId? Delete(ProductRelationshipId relationshipId);
    List<ProductRelationship> FindMatching(Func<ProductRelationship, bool> predicate);
}