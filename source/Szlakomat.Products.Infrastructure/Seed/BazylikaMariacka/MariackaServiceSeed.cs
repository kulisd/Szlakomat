using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Infrastructure.Seed.BazylikaMariacka;

/// <summary>
/// Seeds ancillary services for Bazylika Mariacka as ProductType instances.
///
/// Services modelled:
///   1. Przewodnik po Bazylice — licensed guide service (PL or foreign language)
///      Realistic pricing: ~120 PLN per group for a 45-min interior tour.
///      Requires advance booking; group 1–30 persons.
///   2. Audioprzewodnik Bazylika — audio guide for the interior
///      Available in PL, EN, DE. Realistic rental: 8 PLN/person.
///      Picked up at the ticket office; must be returned after visit.
///   3. Zezwolenie na fotografowanie — photography permit
///      Bazylika allows photography without flash; a small fee applies for
///      professional/commercial photography with tripod or dedicated camera.
///      Casual visitors with smartphones: free.
///      Professional/commercial (tripod, DSLR, film): 50 PLN per session.
///   4. Słuchawki grupowe — group headset rental
///      Required for groups above 10 persons to avoid disturbing other visitors.
///      Realistic: headset sets rented from the ticket office, 5 PLN/headset.
///
/// NOTE: All services are fictionalised realistcally for modelling purposes.
/// The basilica does not publish a full services price list online;
/// prices and exact conditions are approximated from comparable Kraków sites.
/// </summary>
internal static class MariackaServiceSeed
{
    public static List<ProductType> Seed(IProductTypeRepository repo)
    {
        var services = new List<ProductType>
        {
            BuildGuidePl(),
            BuildGuideForeign(),
            BuildAudioGuide(),
            BuildPhotographyPermit(),
            BuildGroupHeadsets(),
        };

        foreach (var service in services)
        {
            repo.Save(service);
        }

        return services;
    }

    // -----------------------------------------------------------------------
    // 1. Licensed guide (Polish)
    // -----------------------------------------------------------------------
    private static ProductType BuildGuidePl()
    {
        // Requires advance booking (> 2 days), group 1–30
        var applicability = ApplicabilityConstraint.And(
            ApplicabilityConstraint.Between("group_size", 1, 30),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 1));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Przewodnik po Bazylice — język polski"),
                ProductDescription.Of(
                    "Usługa licencjonowanego przewodnika w języku polskim. " +
                    "Obejmuje oprowadzanie po wnętrzu Bazyliki Mariackiej: gotycka architektura, " +
                    "ołtarz Wita Stwosza, polichromia Jana Matejki, kaplice boczne. " +
                    "Czas zwiedzania ok. 45 minut. Wymagana rezerwacja min. 2 dni wcześniej."))
            .WithMetadata(SeedHelpers.MariackaBase()
                .With("service_type", "guide")
                .With("guide_language", "PL")
                .With("price_group_pln", "120")
                .With("price_note", "Cena za grupę do 30 osób — niezależna od liczby uczestników")
                .With("min_booking_days_ahead", "2")
                .With("booking_contact", "muzeum@mariacki.com")
                .With("route_covered", "interior"))
            .WithApplicabilityConstraint(applicability)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.BatchTracked)
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("group_size", 1, 30))
                .Build();
    }

    // -----------------------------------------------------------------------
    // 2. Licensed guide (foreign language)
    // -----------------------------------------------------------------------
    private static ProductType BuildGuideForeign()
    {
        var applicability = ApplicabilityConstraint.And(
            ApplicabilityConstraint.Between("group_size", 1, 30),
            ApplicabilityConstraint.GreaterThan("booking_days_ahead", 2));

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Przewodnik po Bazylice — język obcy"),
                ProductDescription.Of(
                    "Usługa licencjonowanego przewodnika w wybranym języku obcym (EN/DE/FR/IT/ES). " +
                    "Oprowadzanie po wnętrzu Bazyliki Mariackiej: ok. 45 minut. " +
                    "Wymagana rezerwacja min. 3 dni wcześniej."))
            .WithMetadata(SeedHelpers.MariackaBase()
                .With("service_type", "guide")
                .With("guide_language", "foreign")
                .With("price_group_pln", "150")
                .With("price_note", "Cena za grupę do 30 osób — obcojęzyczny przewodnik")
                .With("min_booking_days_ahead", "3")
                .With("booking_contact", "muzeum@mariacki.com")
                .With("route_covered", "interior"))
            .WithApplicabilityConstraint(applicability)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.BatchTracked)
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("group_size", 1, 30))
                .WithMandatoryFeature(
                    ProductFeatureType.WithAllowedValues("guide_language", "EN", "DE", "FR", "ES", "IT"))
                .Build();
    }

    // -----------------------------------------------------------------------
    // 3. Audio guide for the interior
    // -----------------------------------------------------------------------
    // Available in PL, EN, DE. Rented from ticket office; returned at exit.
    // Age restriction: must be 12+ to rent independently.
    private static ProductType BuildAudioGuide()
    {
        var ageConstraint = ApplicabilityConstraint.GreaterThan("visitor_age", 11);

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Audioprzewodnik — Bazylika Mariacka"),
                ProductDescription.Of(
                    "Wypożyczenie audioprzewodnika do zwiedzania wnętrza Bazyliki Mariackiej. " +
                    "Dostępny w języku polskim, angielskim i niemieckim. " +
                    "Odbiór i zwrot przy kasie biletowej. Kaucja 20 PLN zwrotna po oddaniu urządzenia."))
            .WithMetadata(SeedHelpers.MariackaBase()
                .With("service_type", "audioguide")
                .With("rental_price_pln", "8")
                .With("deposit_pln", "20")
                .With("deposit_refundable", "true")
                .With("pickup_location", "kasa biletowa, wejście boczne od Placu Mariackiego")
                .With("route_covered", "interior")
                .With("min_age", "12"))
            .WithApplicabilityConstraint(ageConstraint)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .WithMandatoryFeature(
                    ProductFeatureType.WithAllowedValues("audio_language", "PL", "EN", "DE"))
                .Build();
    }

    // -----------------------------------------------------------------------
    // 4. Photography permit (professional/commercial)
    // -----------------------------------------------------------------------
    // Smartphone photography without flash: free (no product needed).
    // Professional photography (tripod, dedicated camera rig, commercial use): paid permit.
    // Applies only to interior; Hejnalica photography is unrestricted.
    private static ProductType BuildPhotographyPermit()
    {
        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Zezwolenie na fotografowanie profesjonalne"),
                ProductDescription.Of(
                    "Zezwolenie na fotografowanie wnętrza Bazyliki Mariackiej z użyciem statywu, " +
                    "lampy studyjnej lub w celach komercyjnych/wydawniczych. " +
                    "Fotografowanie smartfonem bez lampy błyskowej jest bezpłatne. " +
                    "Wymagane złożenie wniosku z min. 5-dniowym wyprzedzeniem."))
            .WithMetadata(SeedHelpers.MariackaBase()
                .With("service_type", "photography_permit")
                .With("price_pln", "50")
                .With("permit_scope", "interior")
                .With("free_photography_note", "Smartfon bez lampy błyskowej — bezpłatnie, bez zezwolenia")
                .With("flash_prohibited", "true")
                .With("application_days_ahead_min", "5")
                .With("booking_contact", "muzeum@mariacki.com"))
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.Identical)
                .WithMandatoryFeature(
                    ProductFeatureType.WithAllowedValues("photography_purpose", "commercial", "publication", "film"))
                .Build();
    }

    // -----------------------------------------------------------------------
    // 5. Group headset rental
    // -----------------------------------------------------------------------
    // Mandatory for groups above 10 persons to avoid disturbing services and other visitors.
    // Rented from ticket office; 5 PLN per headset.
    private static ProductType BuildGroupHeadsets()
    {
        var groupConstraint = ApplicabilityConstraint.GreaterThan("group_size", 10);

        return new ProductBuilder(
                UuidProductIdentifier.New(),
                ProductName.Of("Słuchawki grupowe — Bazylika Mariacka"),
                ProductDescription.Of(
                    "Wypożyczenie zestawów słuchawkowych dla grup powyżej 10 osób. " +
                    "Obowiązkowe w przypadku grup zorganizowanych, aby nie zakłócać ciszy sakralnej " +
                    "i spokoju innych zwiedzających. Odbiór przy kasie biletowej. Cena: 5 PLN/szt."))
            .WithMetadata(SeedHelpers.MariackaBase()
                .With("service_type", "headset_rental")
                .With("price_per_headset_pln", "5")
                .With("required_for_groups_above", "10")
                .With("pickup_location", "kasa biletowa"))
            .WithApplicabilityConstraint(groupConstraint)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.BatchTracked)
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("headset_count", 11, 30))
                .Build();
    }
}