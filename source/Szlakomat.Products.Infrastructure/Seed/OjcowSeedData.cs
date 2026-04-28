using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Relationships;
using Szlakomat.Products.Infrastructure.Catalog;
using Szlakomat.Products.Infrastructure.Seed.OPN;

namespace Szlakomat.Products.Infrastructure.Seed;

/// <summary>
/// Orchestrator that seeds all OPN (Ojcowski Park Narodowy) data.
/// Currently: 9 attractions, 5 trails, 3 packages, 17 catalog entries, 16 relationships.
/// </summary>
internal static class OjcowSeedData
{
    public static void Seed(
        InMemoryProductTypeRepository productRepo,
        InMemoryPackageTypeRepository packageRepo,
        ICatalogEntryRepository catalogRepo,
        IProductRelationshipRepository relationshipRepo,
        ProductRelationshipFactory relationshipFactory)
    {
        var attractions = OPNAttractionsSeed.Seed(productRepo);
        var trails = OPNTrailsSeed.Seed(productRepo);
        var packages = OPNPackagesSeed.Seed(packageRepo, attractions);

        OPNCatalogSeed.Seed(catalogRepo, attractions, trails, packages);
        OPNRelationshipsSeed.Seed(relationshipRepo, relationshipFactory, attractions, trails, packages);
    }
}
