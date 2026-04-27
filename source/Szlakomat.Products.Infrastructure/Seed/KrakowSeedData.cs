using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Relationships;
using Szlakomat.Products.Infrastructure.Catalog;
using Szlakomat.Products.Infrastructure.Seed.BazylikaMariacka;
using Szlakomat.Products.Infrastructure.Seed.MariackaBasilica;
using Szlakomat.Products.Infrastructure.Seed.Wawel;

namespace Szlakomat.Products.Infrastructure.Seed;

/// <summary>
/// Orchestrator that seeds Kraków attractions data.
///
/// Included:
/// - Wawel Royal Castle
/// - Bazylika Mariacka
///
/// Easy to extend with more Kraków attractions in future.
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
        SeedWawel(
            productRepo,
            packageRepo,
            catalogRepo,
            relationshipRepo,
            relationshipFactory);

        SeedMariacka(
            productRepo,
            packageRepo,
            catalogRepo,
            relationshipRepo,
            relationshipFactory);
    }

    private static void SeedWawel(
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
        WawelRelationshipsSeed.Seed(
            relationshipRepo,
            relationshipFactory,
            exhibitions,
            services,
            packages);
    }

    private static void SeedMariacka(
        InMemoryProductTypeRepository productRepo,
        InMemoryPackageTypeRepository packageRepo,
        ICatalogEntryRepository catalogRepo,
        IProductRelationshipRepository relationshipRepo,
        ProductRelationshipFactory relationshipFactory)
    {
        var exhibitions = MariackaExhibitionsSeed.Seed(productRepo);
        var services = MariackaServicesSeed.Seed(productRepo);

        var packages = MariackaPackagesSeed.Seed(
            packageRepo,
            exhibitions.BasilikaInterior,
            exhibitions.Hejnalica,
            services);

        MariackkaCatalogSeed.Seed(
            catalogRepo,
            exhibitions.BasilikaInterior,
            exhibitions.Hejnalica);

        MariackaRelationshipsSeed.Seed(
            relationshipRepo,
            relationshipFactory,
            exhibitions.BasilikaInterior,
            exhibitions.Hejnalica);
    }
}