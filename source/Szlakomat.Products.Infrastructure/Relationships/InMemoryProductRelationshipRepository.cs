using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Infrastructure.Relationships;

/// <summary>
/// In-memory implementation of ProductRelationshipRepository using thread-safe storage.
/// </summary>
internal class InMemoryProductRelationshipRepository : IProductRelationshipRepository
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<ProductRelationshipId, ProductRelationship> _map
        = new();

    public List<ProductRelationship> FindAllRelationsFrom(IProductIdentifier productIdentifier)
    {
        return _map.Values
            .Where(rel => rel.From.Equals(productIdentifier))
            .ToList();
    }

    public List<ProductRelationship> FindAllRelationsFrom(IProductIdentifier productIdentifier, ProductRelationshipType type)
    {
        return _map.Values
            .Where(rel => rel.From.Equals(productIdentifier) && rel.Type == type)
            .ToList();
    }

    public ProductRelationship? FindBy(ProductRelationshipId relationshipId)
    {
        return _map.TryGetValue(relationshipId, out var value) ? value : null;
    }

    public void Save(ProductRelationship productRelationship)
    {
        _map[productRelationship.Id] = productRelationship;
    }

    public ProductRelationshipId? Delete(ProductRelationshipId relationshipId)
    {
        if (_map.TryRemove(relationshipId, out var removed))
        {
            return removed.Id;
        }
        return null;
    }

    public List<ProductRelationship> FindMatching(Func<ProductRelationship, bool> predicate)
    {
        return _map.Values
            .Where(predicate)
            .ToList();
    }
}
