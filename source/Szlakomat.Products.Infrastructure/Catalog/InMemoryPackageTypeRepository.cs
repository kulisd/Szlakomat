using System.Collections.Concurrent;
using Szlakomat.Products.Domain.Catalog.PackageType;

namespace Szlakomat.Products.Infrastructure.Catalog;

internal class InMemoryPackageTypeRepository : IPackageTypeRepository
{
    private readonly ConcurrentDictionary<string, PackageType> _storage = new();

    public void Save(PackageType packageType)
    {
        _storage[packageType.Id().ToString()!] = packageType;
    }

    public PackageType? FindByIdValue(string idValue)
    {
        return _storage.TryGetValue(idValue, out var value) ? value : null;
    }
}
