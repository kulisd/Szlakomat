using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Relationships;
using Szlakomat.Products.Infrastructure.Catalog;
using Szlakomat.Products.Infrastructure.Seed.Wawel;

namespace Szlakomat.Products.Infrastructure.Seed;

/// <summary>
/// Orchestrator that seeds all Kraków attraction data.
/// Currently: Wawel Royal Castle (13 exhibitions, 5 services, 8 packages,
/// 26 catalog entries, 19 product relationships).
/// </summary>
internal static class KrakowSeedData
{
    public static void Seed(
        InMemoryProductTypeRepository productRepo,
        InMemoryPackageTypeRepository packageRepo,
        ICatalogEntryRepository catalogRepo,
        IProductRelationshipRepository relationshipRepo,
        ProductRelationshipFactory relationshipFactory)
    {
        var exhibitions = WawelExhibitionsSeed.Seed(productRepo);
        var services = WawelServicesSeed.Seed(productRepo);
        var packages = WawelPackagesSeed.Seed(packageRepo, exhibitions, services);

        WawelCatalogSeed.Seed(catalogRepo, exhibitions, services, packages);
        WawelRelationshipsSeed.Seed(relationshipRepo, relationshipFactory, exhibitions, services, packages);
    }
}
