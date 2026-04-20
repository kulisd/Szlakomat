using System.Collections.Concurrent;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Infrastructure.CommercialOffer;

/// <summary>
/// In-memory implementation of CatalogEntryRepository using thread-safe storage.
/// </summary>
internal class InMemoryCatalogEntryRepository : ICatalogEntryRepository
{
    private readonly ConcurrentDictionary<CatalogEntryId, CatalogEntry> _storage = new();

    public void Save(CatalogEntry entry)
    {
        _storage[entry.Id()] = entry;
    }

    public CatalogEntry? FindById(CatalogEntryId id)
    {
        return _storage.TryGetValue(id, out var value) ? value : null;
    }

    public IReadOnlySet<CatalogEntry> FindAll()
    {
        return new HashSet<CatalogEntry>(_storage.Values);
    }

    public IReadOnlySet<CatalogEntry> FindByProductType(IProductIdentifier productTypeId)
    {
        return _storage.Values
            .Where(entry => entry.Product().Id().Equals(productTypeId))
            .ToHashSet();
    }

    public IReadOnlySet<CatalogEntry> FindByCategory(string category)
    {
        return _storage.Values
            .Where(entry => entry.IsInCategory(category))
            .ToHashSet();
    }

    public void Remove(CatalogEntryId id)
    {
        _storage.TryRemove(id, out _);
    }
}
