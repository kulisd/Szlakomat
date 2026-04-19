using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.Catalog.ProductType;

/// <summary>
/// Repository for ProductType persistence.
/// </summary>
internal interface IProductTypeRepository
{
    void Save(ProductType productType);
    ProductType? FindById(IProductIdentifier id);
    ProductType? FindByIdValue(string idValue);
    IReadOnlySet<ProductType> FindAll();
    IReadOnlySet<ProductType> FindByTrackingStrategy(ProductTrackingStrategy strategy);
    void Remove(IProductIdentifier id);
}
