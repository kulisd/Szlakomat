using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Relationships;
using Szlakomat.Products.Infrastructure.Catalog;
using Szlakomat.Products.Infrastructure.Seed.KopiecKosciuszki;
using Szlakomat.Products.Infrastructure.Seed.Wawel;

namespace Szlakomat.Products.Infrastructure.Seed;

/// <summary>
/// Orchestrator that seeds all Kraków attraction data.
/// Currently:
///   - Wawel Royal Castle (13 exhibitions, 5 services, 8 packages, 26 catalog entries, 19 relationships)
///   - Kopiec Kościuszki (5 attractions, 4 services, 6 packages, ~18 relationships)
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
        // --- Wawel Royal Castle ---
        var wawelExhibitions = WawelExhibitionsSeed.Seed(productRepo);
        var wawelServices = WawelServicesSeed.Seed(productRepo);
        var wawelPackages = WawelPackagesSeed.Seed(packageRepo, wawelExhibitions, wawelServices);

        WawelCatalogSeed.Seed(catalogRepo, wawelExhibitions, wawelServices, wawelPackages);
        WawelRelationshipsSeed.Seed(relationshipRepo, relationshipFactory, wawelExhibitions, wawelServices, wawelPackages);

        // --- Kopiec Kościuszki ---
        var kopiecAttractions = KopiecKosciuszkiExhibitionsSeed.Seed(productRepo);
        var kopiecServices = KopiecKosciuszkiServicesSeed.Seed(productRepo);
        var kopiecPackages = KopiecKosciuszkiPackagesSeed.Seed(packageRepo, kopiecAttractions, kopiecServices);

        KopiecKosciuszkiCatalogSeed.Seed(catalogRepo, kopiecAttractions, kopiecServices, kopiecPackages);
        KopiecKosciuszkiRelationshipsSeed.Seed(relationshipRepo, relationshipFactory, kopiecAttractions, kopiecServices, kopiecPackages);
    }
}
