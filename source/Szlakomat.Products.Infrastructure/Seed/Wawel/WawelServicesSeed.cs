using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Infrastructure.Seed.Wawel;

/// <summary>
/// Seeds 5 Wawel services as ProductType instances.
/// Guide prices aligned with wawel.krakow.pl cennik od 01.01.2025.
/// </summary>
internal static class WawelServicesSeed
{
    public static List<ProductType> Seed(IProductTypeRepository repo)
    {
        var services = new List<ProductType>
        {
            BuildGuidePl(),
            BuildGuideForeign(),
            BuildAudioGuide(),
            BuildLuggageStorage(),
            BuildHeadsetRental(),
        };

        foreach (var service in services)
        {
            repo.Save(service);
        }

        return services;
    }

    private static ProductType BuildGuidePl()
    {
        // Guide PL: BatchTracked, requires 14-day advance booking, group 1-30
        var guideApplicability = ApplicabilityConstraint.And(
            ApplicabilityConstraint.Between("group_size", 1, 30),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 13));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Przewodnik licencjonowany — język polski"),
                ProductDescription.Of("Usługa przewodnika licencjonowanego w języku polskim po wystawach Zamku Królewskiego na Wawelu."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("service_type", "guide")
                .With("guide_language", "PL")
                .With("price_1_exhibition_pln", "160")
                .With("price_2_exhibitions_pln", "320")
                .With("price_3_exhibitions_pln", "480")
                .With("price_4_exhibitions_pln", "640")
                .With("price_5_exhibitions_pln", "800")
                .With("price_valid_from", "2025-01-01"))
            .WithApplicabilityConstraint(guideApplicability)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.BatchTracked)
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("group_size", 1, 30))
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("num_exhibitions", 1, 5))
                .Build();
    }

    private static ProductType BuildGuideForeign()
    {
        var guideApplicability = ApplicabilityConstraint.And(
            ApplicabilityConstraint.Between("group_size", 1, 30),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 13));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Przewodnik licencjonowany — język obcy"),
                ProductDescription.Of("Usługa przewodnika licencjonowanego w języku obcym po wystawach Zamku Królewskiego na Wawelu."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("service_type", "guide")
                .With("guide_language", "foreign")
                .With("price_1_exhibition_pln", "190")
                .With("price_2_exhibitions_pln", "380")
                .With("price_3_exhibitions_pln", "570")
                .With("price_4_exhibitions_pln", "760")
                .With("price_5_exhibitions_pln", "950")
                .With("price_valid_from", "2025-01-01"))
            .WithApplicabilityConstraint(guideApplicability)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.BatchTracked)
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("group_size", 1, 30))
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("num_exhibitions", 1, 5))
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("guide_language", "EN", "DE", "FR", "ES", "IT", "UA"))
                .Build();
    }

    private static ProductType BuildAudioGuide()
    {
        // Age > 12 (GreaterThan 11)
        var ageConstraint = ApplicabilityConstraint.GreaterThan("visitor_age", 11);

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Audioprzewodnik"),
                ProductDescription.Of("Wypożyczenie audioprzewodnika do zwiedzania wystaw zamkowych."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("service_type", "audioguide")
                .With("rental_price_pln", "14")
                .With("provider", "Movitech")
                .With("pjm_available_exhibitions", "Prywatne Apartamenty Królewskie, Reprezentacyjne Komnaty Królewskie, Skarbiec Koronny, Podziemia Zamku")
                .With("pjm_extra_duration_min", "20")
                .With("pickup_zamek_p1_p2", "wejście na klatkę Poselską")
                .With("pickup_podziemia", "wejście na Wawel Zaginiony")
                .With("pickup_skarbiec", "przy wejściu")
                .With("pickup_zbrojownia", "przy wejściu")
                .With("pickup_miedzymurze", "przy wejściu"))
            .WithApplicabilityConstraint(ageConstraint)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .WithMandatoryFeature(
                    ProductFeatureType.WithAllowedValues("audio_language", "PL", "EN", "UA", "DE", "FR", "IT", "ES", "PJM"))
                .Build();
    }

    private static ProductType BuildLuggageStorage()
    {
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Przechowalnia bagażu"),
                ProductDescription.Of("Obowiązkowa przechowalnia bagażu na dziedzińcu arkadowym."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("service_type", "luggage_storage")
                .With("location", "arcaded_courtyard"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();
    }

    private static ProductType BuildHeadsetRental()
    {
        // Required for groups > 8 people
        var groupConstraint = ApplicabilityConstraint.GreaterThan("group_size", 8);

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Wypożyczenie słuchawek grupowych"),
                ProductDescription.Of("Wypożyczenie zestawów słuchawkowych dla grup powyżej 8 osób."))
            .WithMetadata(SeedHelpers.WawelBase()
                .With("service_type", "headset_rental")
                .With("required_for_groups_above", "8"))
            .WithApplicabilityConstraint(groupConstraint)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.BatchTracked)
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("headset_count", 9, 30))
                .Build();
    }
}
