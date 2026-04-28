using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Infrastructure.Seed.KopiecKosciuszki;

/// <summary>
/// Seeds 4 Kopiec Kościuszki services as ProductType instances.
/// </summary>
internal static class KopiecKosciuszkiServicesSeed
{
    public static List<ProductType> Seed(IProductTypeRepository repo)
    {
        var services = new List<ProductType>
        {
            BuildGuidePl(),
            BuildGuideForeign(),
            BuildAudioGuide(),
            BuildLuggageLockers(),
        };

        foreach (var service in services)
        {
            repo.Save(service);
        }

        return services;
    }

    private static ProductType BuildGuidePl()
    {
        // Przewodnik wymaga rezerwacji min. 7 dni przed wizytą, grupy 1-25 osób
        var guideApplicability = ApplicabilityConstraint.And(
            ApplicabilityConstraint.Between("group_size", 1, 25),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 6));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Przewodnik — język polski"),
                ProductDescription.Of("Usługa licencjonowanego przewodnika po Kopcu Kościuszki i forcie w języku polskim."))
            .WithMetadata(SeedHelpers.KopiecBase()
                .With("service_type", "guide")
                .With("guide_language", "PL")
                .With("price_1h_pln", "120")
                .With("price_2h_pln", "200")
                .With("price_valid_from", "2025-01-01"))
            .WithApplicabilityConstraint(guideApplicability)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.BatchTracked)
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("group_size", 1, 25))
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("tour_duration_h", "1", "2"))
                .Build();
    }

    private static ProductType BuildGuideForeign()
    {
        var guideApplicability = ApplicabilityConstraint.And(
            ApplicabilityConstraint.Between("group_size", 1, 25),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 6));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Przewodnik — język obcy"),
                ProductDescription.Of("Usługa licencjonowanego przewodnika po Kopcu Kościuszki i forcie w języku obcym (EN/DE/UA/FR)."))
            .WithMetadata(SeedHelpers.KopiecBase()
                .With("service_type", "guide")
                .With("guide_language", "foreign")
                .With("price_1h_pln", "150")
                .With("price_2h_pln", "250")
                .With("price_valid_from", "2025-01-01"))
            .WithApplicabilityConstraint(guideApplicability)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.BatchTracked)
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("group_size", 1, 25))
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("tour_duration_h", "1", "2"))
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("guide_language", "EN", "DE", "UA", "FR"))
                .Build();
    }

    private static ProductType BuildAudioGuide()
    {
        // Audioprzewodnik dla odwiedzających pow. 12. roku życia
        var ageConstraint = ApplicabilityConstraint.GreaterThan("visitor_age", 11);

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Audioprzewodnik Kopiec Kościuszki"),
                ProductDescription.Of("Wypożyczenie audioprzewodnika z trasą po kopcu i muzeum."))
            .WithMetadata(SeedHelpers.KopiecBase()
                .With("service_type", "audioguide")
                .With("rental_price_pln", "10")
                .With("available_languages", "PL, EN, DE")
                .With("pickup_location", "kasa biletowa przy wejściu do fortu"))
            .WithApplicabilityConstraint(ageConstraint)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .WithMandatoryFeature(
                    ProductFeatureType.WithAllowedValues("audio_language", "PL", "EN", "DE"))
                .Build();
    }

    private static ProductType BuildLuggageLockers()
    {
        // Szafki bagażowe przy kasie — bezobsługowe, płatne monetą
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Szafka bagażowa"),
                ProductDescription.Of("Samoobsługowa szafka bagażowa przy kasie biletowej fortu. Wymagana moneta 5 PLN (zwrotna)."))
            .WithMetadata(SeedHelpers.KopiecBase()
                .With("service_type", "luggage_locker")
                .With("deposit_pln", "5")
                .With("deposit_refundable", "true")
                .With("location", "kasa biletowa"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .Build();
    }
}
