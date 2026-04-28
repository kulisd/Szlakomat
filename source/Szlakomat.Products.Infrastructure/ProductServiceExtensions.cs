using Microsoft.Extensions.DependencyInjection;
using Szlakomat.Products.Application;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Instances;
using Szlakomat.Products.Domain.Relationships;
using Szlakomat.Products.Infrastructure.Catalog;
using Szlakomat.Products.Infrastructure.CommercialOffer;
using Szlakomat.Products.Infrastructure.Instances;
using Szlakomat.Products.Infrastructure.Relationships;
using Szlakomat.Products.Infrastructure.Seed;

namespace Szlakomat.Products.Infrastructure;

public static class ProductServiceExtensions
{
    public static void AddProductModule(this IServiceCollection services)
    {
        var productRepo = new InMemoryProductTypeRepository();
        var packageRepo = new InMemoryPackageTypeRepository();
        var catalogRepo = new InMemoryCatalogEntryRepository();
        var relationshipRepo = new InMemoryProductRelationshipRepository();
        var relationshipFactory = new ProductRelationshipFactory(ProductRelationshipId.Random);

        KrakowSeedData.Seed(productRepo, packageRepo, catalogRepo, relationshipRepo, relationshipFactory);
        OjcowSeedData.Seed(productRepo, packageRepo, catalogRepo, relationshipRepo, relationshipFactory);

        services.AddSingleton<IProductTypeRepository>(productRepo);
        services.AddSingleton<IPackageTypeRepository>(packageRepo);
        services.AddSingleton<ICatalogEntryRepository>(catalogRepo);
        services.AddSingleton<IProductRelationshipRepository>(relationshipRepo);
        services.AddSingleton<IInstanceRepository>(new InMemoryInstanceRepository());
        services.AddSingleton(relationshipFactory);

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<ProductModule>());
    }
}
