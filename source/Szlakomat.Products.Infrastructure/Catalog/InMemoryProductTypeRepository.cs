using System.Collections.Concurrent;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Infrastructure.Catalog;

/// <summary>
/// In-memory implementation of ProductTypeRepository using thread-safe storage.
/// </summary>
internal class InMemoryProductTypeRepository : IProductTypeRepository
{
    private readonly ConcurrentDictionary<IProductIdentifier, ProductType> _storage = new();

    public void Save(ProductType productType)
    {
        _storage[productType.Id()] = productType;
    }

    public ProductType? FindById(IProductIdentifier id)
    {
        return _storage.TryGetValue(id, out var value) ? value : null;
    }

    public ProductType? FindByIdValue(string idValue)
    {
        return _storage.Values.FirstOrDefault(pt => pt.Id().ToString() == idValue);
    }

    public IReadOnlySet<ProductType> FindAll()
    {
        return new HashSet<ProductType>(_storage.Values);
    }

    public IReadOnlySet<ProductType> FindByTrackingStrategy(ProductTrackingStrategy strategy)
    {
        return _storage.Values
            .Where(pt => pt.TrackingStrategy() == strategy)
            .ToHashSet();
    }

    public void Remove(IProductIdentifier id)
    {
        _storage.TryRemove(id, out _);
    }
}
