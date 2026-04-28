using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Infrastructure.Seed.OPN;

/// <summary>
/// Seeds Ojcowski Park Narodowy attractions as ProductType instances.
/// Data source: Kompendium Atrakcji i Szlaków OPN V5.
/// </summary>
internal static class OPNAttractionsSeed
{
    private static readonly ProductFeatureType TicketType =
        ProductFeatureType.WithAllowedValues("ticket_type", "standard", "ulgowy");

    private static readonly ProductFeatureType VisitDate =
        ProductFeatureType.WithDateRange("visit_date", "2025-01-01", "2026-12-31");

    // Guided cave tours require language selection
    private static readonly ProductFeatureType GuideLanguage =
        ProductFeatureType.WithAllowedValues("guide_language", "PL", "EN");

    public static List<ProductType> Seed(IProductTypeRepository repo)
    {
        var attractions = new List<ProductType>
        {
            BuildZamekKazimierzowski(),
            BuildJaskiniaLokietka(),
            BuildJaskiniaCiemna(),
            BuildBramaKrakowska(),
            BuildZamekPieskowaSkala(),
            BuildMaczugaHerkulesa(),
            BuildKaplicaNaWodzie(),
            BuildGrodzisko(),
            BuildMlynBoronia(),
        };

        foreach (var attraction in attractions)
        {
            repo.Save(attraction);
        }

        return attractions;
    }

    private static ProductType BuildZamekKazimierzowski() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"),
                ProductName.Of("Zamek Kazimierzowski w Ojcowie"),
                ProductDescription.Of("XIV-wieczna warownia Kazimierza Wielkiego. Malownicze ruiny z neogotycką wieżą bramną i ekspozycją. Z tarasów widok na Dolinę Prądnika."))
            .WithMetadata(SeedHelpers.OPNAttractionBase()
                .With("category", "zamek-ruiny")
                .With("price_standard_pln", "22")
                .With("price_reduced_pln", "15")
                .With("payment_cash_only", "true")
                .With("accessibility", "partial")
                .With("nearby", "Ogród Ojcowski przy willi Jadwiga")
                .With("avg_duration_min", "60"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.IndividuallyTracked)
                .WithMandatoryFeature(TicketType)
                .WithMandatoryFeature(VisitDate)
                .Build();

    private static ProductType BuildJaskiniaLokietka() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"),
                ProductName.Of("Jaskinia Łokietka"),
                ProductDescription.Of("Legendarne schronienie króla Władysława Łokietka — charakterystyczna pajęczynowa krata. Temperatura stała 7–8°C przez cały rok. Zwiedzanie wyłącznie z przewodnikiem."))
            .WithMetadata(SeedHelpers.OPNAttractionBase()
                .With("category", "jaskinia")
                .With("temperature_celsius", "7-8")
                .With("slippery_floor", "true")
                .With("warm_clothing_required", "true")
                .With("hooded_clothing_recommended", "true")
                .With("sturdy_footwear_required", "true")
                .With("parking_czajowice_m", "300")
                .With("connected_trail", "niebieski")
                .With("guided_tour_only", "true")
                .With("avg_duration_min", "40"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.IndividuallyTracked)
                .WithMandatoryFeature(TicketType)
                .WithMandatoryFeature(VisitDate)
                .WithMandatoryFeature(GuideLanguage)
                .Build();

    private static ProductType BuildJaskiniaCiemna() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"),
                ProductName.Of("Jaskinia Ciemna"),
                ProductDescription.Of("Obozowisko neandertalczyków sprzed 50 tys. lat. Największa komora wstępna na Wyżynie. Brak oświetlenia elektrycznego — przewodnik ma lampę, warto mieć własne źródło światła."))
            .WithMetadata(SeedHelpers.OPNAttractionBase()
                .With("category", "jaskinia")
                .With("no_electric_lighting", "true")
                .With("own_light_recommended", "true")
                .With("steep_approach_via_trail", "zielony")
                .With("slippery_when_wet", "true")
                .With("location", "nad Bramą Krakowską")
                .With("guided_tour_only", "true")
                .With("avg_duration_min", "40"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.IndividuallyTracked)
                .WithMandatoryFeature(TicketType)
                .WithMandatoryFeature(VisitDate)
                .WithMandatoryFeature(GuideLanguage)
                .Build();

    // Free, flat terrain — most accessible spot in OPN for strollers and wheelchairs
    private static ProductType BuildBramaKrakowska() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"),
                ProductName.Of("Brama Krakowska, Jonaszówka i Źródło Miłości"),
                ProductDescription.Of("Skalna brama (15 m) i wywierzysko krasowe z basenem w kształcie serca. Skała Jonaszówka — główny punkt widokowy doliny. Płaski, utwardzony teren — jedyne miejsce w OPN dostępne dla wózków."))
            .WithMetadata(SeedHelpers.OPNAttractionBase()
                .With("price_standard_pln", "0")
                .With("accessibility", "full")
                .With("prams_accessible", "true")
                .With("family_friendly", "true")
                .With("viewpoint", "Skała Jonaszówka")
                .With("nearby_gastronomy", "pstrąg ojcowski — stawy hodowlane przy Jonaszówce")
                .With("avg_duration_min", "30"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();

    private static ProductType BuildZamekPieskowaSkala() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"),
                ProductName.Of("Zamek w Pieskowej Skale"),
                ProductDescription.Of("Perła renesansu z arkadowym dziedzińcem i muzeum. Północna brama OPN. U stóp wzgórza zabytkowa willa Chopin i stawy (szlak czarny)."))
            .WithMetadata(SeedHelpers.OPNAttractionBase()
                .With("category", "zamek-muzeum")
                .With("style", "renesans")
                .With("indoor_outdoor", "mixed")
                .With("northern_gate_of_park", "true")
                .With("nearby", "willa Chopin, stawy — szlak czarny")
                .With("avg_duration_min", "90"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.IndividuallyTracked)
                .WithMandatoryFeature(TicketType)
                .WithMandatoryFeature(VisitDate)
                .Build();

    private static ProductType BuildMaczugaHerkulesa() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"),
                ProductName.Of("Maczuga Herkulesa"),
                ProductDescription.Of("Monumentalna 25-metrowa ostańcowa skała wapienna — naturalna wizytówka geologiczna OPN. Dostępna bezpłatnie, widoczna ze szlaku czarnego."))
            .WithMetadata(SeedHelpers.OPNAttractionBase()
                .With("price_standard_pln", "0")
                .With("height_m", "25")
                .With("visible_from_trail", "czarny")
                .With("avg_duration_min", "15"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();

    private static ProductType BuildKaplicaNaWodzie() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("a7b8c9d0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"),
                ProductName.Of("Kaplica na Wodzie"),
                ProductDescription.Of("Drewniana kaplica wybudowana na palach nad rzeką Prądnik — unikalna konstrukcja omijająca XIX-wieczny zakaz budowy świątyń na ziemi. Widoczna ze szlaku czerwonego."))
            .WithMetadata(SeedHelpers.OPNAttractionBase()
                .With("category", "obiekt-sakralny")
                .With("price_standard_pln", "0")
                .With("visible_from_trail", "czerwony")
                .With("construction_type", "na palach")
                .With("avg_duration_min", "15"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();

    private static ProductType BuildGrodzisko() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("b8c9d0e1-f2a3-4b4c-5d6e-7f8a9b0c1d2e"),
                ProductName.Of("Grodzisko — Kompleks Salomei"),
                ProductDescription.Of("Kompleks klasztorny z rzeźbą słonia i relikwiariem bł. Salomei. Idealne miejsce na wyciszenie z dala od głównego potoku turystów w Ojcowie."))
            .WithMetadata(SeedHelpers.OPNAttractionBase()
                .With("category", "obiekt-sakralny")
                .With("price_standard_pln", "0")
                .With("crowd_avoidance", "true")
                .With("sculpture", "rzeźba słonia")
                .With("avg_duration_min", "30"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();

    // Interior accessible only by prior phone appointment
    private static ProductType BuildMlynBoronia() =>
        new ProductBuilder(
                UuidProductIdentifier.Of("c9d0e1f2-a3b4-4c5d-6e7f-8a9b0c1d2e3f"),
                ProductName.Of("Młyn Boronia"),
                ProductDescription.Of("Najlepiej zachowany zabytek techniki przemysłowej w OPN. Wnętrza dostępne wyłącznie po wcześniejszym telefonicznym umówieniu."))
            .WithMetadata(SeedHelpers.OPNAttractionBase()
                .With("category", "zabytek-techniki")
                .With("indoor_outdoor", "mixed")
                .With("interior_by_appointment_only", "true")
                .With("avg_duration_min", "30"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.BatchTracked)
                .WithMandatoryFeature(VisitDate)
                .Build();
}
