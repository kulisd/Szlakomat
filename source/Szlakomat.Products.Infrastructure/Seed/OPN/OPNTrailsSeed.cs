using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Infrastructure.Seed.OPN;

/// <summary>
/// Seeds OPN hiking trails as ProductType instances.
/// All trails are free (Identical tracking, no mandatory features).
/// Data source: Kompendium Atrakcji i Szlaków OPN V5.
/// </summary>
internal static class OPNTrailsSeed
{
    public static List<ProductType> Seed(IProductTypeRepository repo)
    {
        var trails = new List<ProductType>
        {
            BuildTrailCzerwony(),
            BuildTrailNiebieski(),
            BuildTrailZielony(),
            BuildTrailZolty(),
            BuildTrailCzarny(),
        };

        foreach (var trail in trails)
        {
            repo.Save(trail);
        }

        return trails;
    }

    private static ProductType BuildTrailCzerwony() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("d0e1f2a3-b4c5-4d6e-7f8a-9b0c1d2e3f4a"),
                ProductName.Of("Szlak Orlich Gniazd (czerwony)"),
                ProductDescription.Of("Główna oś OPN (13,6 km), w większości asfalt dnem doliny. Jedyny szlak rekomendowany dla wózków inwalidzkich i dziecięcych — odcinek Ojców–Brama Krakowska."))
            .WithMetadata(SeedHelpers.OPNTrailBase()
                .With("color", "czerwony")
                .With("length_km", "13.6")
                .With("surface", "asfalt")
                .With("difficulty", "łatwy")
                .With("wheelchair_accessible", "true")
                .With("wheelchair_section", "Ojców–Brama Krakowska"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();

    private static ProductType BuildTrailNiebieski() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("e1f2a3b4-c5d6-4e7f-8a9b-0c1d2e3f4a5b"),
                ProductName.Of("Szlak Warowni Jurajskich (niebieski)"),
                ProductDescription.Of("Przez Wąwóz Ciasne Skałki, łączy Jaskinię Łokietka z Bramą Krakowską. Duże nachylenie — wymagane obuwie z profilowanym bieżnikiem."))
            .WithMetadata(SeedHelpers.OPNTrailBase()
                .With("color", "niebieski")
                .With("terrain", "wąwóz")
                .With("difficulty", "średni")
                .With("steep", "true")
                .With("profiled_footwear_required", "true")
                .With("connects", "Jaskinia Łokietka, Brama Krakowska"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();

    private static ProductType BuildTrailZielony() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("f2a3b4c5-d6e7-4f8a-9b0c-1d2e3f4a5b6c"),
                ProductName.Of("Szlak Park Zamkowy – Jaskinia Ciemna (zielony)"),
                ProductDescription.Of("Widokowy szlak przez szczyty Góry Koronnej. Charakter górski, strome podejścia. Odradzany dla osób z lękiem wysokości i rodzin z wózkami."))
            .WithMetadata(SeedHelpers.OPNTrailBase()
                .With("color", "zielony")
                .With("difficulty", "trudny")
                .With("mountain_character", "true")
                .With("steep", "true")
                .With("not_recommended_vertigo", "true")
                .With("not_recommended_strollers", "true")
                .With("viewpoints", "Góra Koronna"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();

    private static ProductType BuildTrailZolty() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("a3b4c5d6-e7f8-4a9b-0c1d-2e3f4a5b6c7d"),
                ProductName.Of("Szlak Dolina Sąspowska (żółty)"),
                ProductDescription.Of("Trasa przez Dolinę Sąspowską — najlepsza na ucieczkę od tłumów. Teren bywa podmokły, zalecane obuwie wodoodporne."))
            .WithMetadata(SeedHelpers.OPNTrailBase()
                .With("color", "żółty")
                .With("difficulty", "łatwy-średni")
                .With("terrain", "podmokły")
                .With("crowd_avoidance", "true")
                .With("waterproof_footwear_recommended", "true"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();

    private static ProductType BuildTrailCzarny() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("b4c5d6e7-f8a9-4b0c-1d2e-3f4a5b6c7d8e"),
                ProductName.Of("Szlak Łącznik Widokowy (czarny)"),
                ProductDescription.Of("Łącznik widokowy. Kluczowy punkt: Skała Jonaszówka — stąd najlepsza panorama Ojcowa. U stóp wzgórza Pieskowej Skały: stawy i willa Chopin."))
            .WithMetadata(SeedHelpers.OPNTrailBase()
                .With("color", "czarny")
                .With("difficulty", "średni")
                .With("key_viewpoint", "Skała Jonaszówka")
                .With("nearby_pieskowa_skala", "stawy, willa Chopin"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();
}
