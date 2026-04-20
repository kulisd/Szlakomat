namespace Szlakomat.Products.Domain.Catalog.PackageType;

internal interface IPackageTypeRepository
{
    void Save(PackageType packageType);
    PackageType? FindByIdValue(string idValue);
}
