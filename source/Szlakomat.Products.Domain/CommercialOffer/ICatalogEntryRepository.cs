using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.CommercialOffer;

/// <summary>
/// Repository for CatalogEntry persistence.
/// </summary>
internal interface ICatalogEntryRepository
{
    void Save(CatalogEntry entry);
    CatalogEntry? FindById(CatalogEntryId id);
    IReadOnlySet<CatalogEntry> FindAll();
    IReadOnlySet<CatalogEntry> FindByProductType(IProductIdentifier productTypeId);
    IReadOnlySet<CatalogEntry> FindByCategory(string category);
    void Remove(CatalogEntryId id);
}