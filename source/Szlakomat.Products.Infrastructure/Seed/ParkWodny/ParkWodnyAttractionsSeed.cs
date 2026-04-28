using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Infrastructure.Seed.ParkWodny;

/// <summary>
/// Seeds Park Wodny Kraków attractions as ProductType instances.
/// Aligned with parkwodny.pl (stan na 2025).
/// Adres: ul. Dobrego Pasterza 126, 31-416 Kraków.
/// </summary>
internal static class ParkWodnyAttractionsSeed
{
    // Shared feature definitions
    private static readonly ProductFeatureType TicketType =
        ProductFeatureType.WithAllowedValues("ticket_type", "normalny", "ulgowy", "dziecięcy", "senior", "rodzinny");

    private static readonly ProductFeatureType VisitDate =
        ProductFeatureType.WithDateRange("visit_date", "2025-01-01", "2026-12-31");

    private static readonly ProductFeatureType VisitTimeSlot =
        ProductFeatureType.WithAllowedValues("time_slot",
            "10:00", "11:00", "12:00", "13:00", "14:00",
            "15:00", "16:00", "17:00", "18:00", "19:00");

    private static readonly ProductFeatureType DurationHours =
        ProductFeatureType.WithAllowedValues("duration_hours", "1", "2", "3", "cały_dzień");

    // Park Wodny is open year-round (Mon–Thu 10–22, Fri 10–23, Sat 9–23, Sun 9–22)
    private static readonly IApplicabilityConstraint StandardAvailability =
        ApplicabilityConstraint.AlwaysTrue();

    // Strefa SPA / sauna — min. 16 lat (GreaterThan 15)
    private static readonly IApplicabilityConstraint AdultOnly =
        ApplicabilityConstraint.GreaterThan("visitor_age", 15);

    public static List<ProductType> Seed(IProductTypeRepository repo)
    {
        var attractions = new List<ProductType>
        {
            // ═══════════════════════════════════════════════
            // STREFA AQUA PARK (atrakcje dostępne dla ogółu)
            // ═══════════════════════════════════════════════

            // --- Basen Rekreacyjny ---
            BuildAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0001-4000-8000-000000000001"),
                "Basen Rekreacyjny",
                "Duży basen rekreacyjny (25 m) z falą morską i jacuzzi. Dostępny dla wszystkich gości parku wodnego.",
                zone: "aqua_park", indoorOutdoor: "indoor", avgDuration: "120",
                extraMetadata: m => m
                    .With("pool_length_m", "25")
                    .With("wave_machine", "true")
                    .With("water_temp_c", "29")
                    .With("jacuzzi", "true")
                    .With("price_1h_normalny_pln", "22")
                    .With("price_1h_ulgowy_pln", "17")),

            // --- Basen Sportowy ---
            BuildAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0002-4000-8000-000000000002"),
                "Basen Sportowy",
                "6-torowy basen sportowy (25 m) do pływania rekreacyjnego i treningowego.",
                zone: "aqua_park", indoorOutdoor: "indoor", avgDuration: "90",
                extraMetadata: m => m
                    .With("pool_length_m", "25")
                    .With("lanes", "6")
                    .With("water_temp_c", "27")
                    .With("price_1h_normalny_pln", "22")
                    .With("price_1h_ulgowy_pln", "17")),

            // --- Zjeżdżalnie Wodne (4 tory) ---
            BuildAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0003-4000-8000-000000000003"),
                "Zjeżdżalnie Wodne",
                "Cztery zjeżdżalnie wodne różnych typów: rura, wąż, kamikaze oraz rodzinna. Dostępne dla dzieci min. 120 cm wzrostu (dorosły z dzieckiem poniżej 120 cm).",
                zone: "aqua_park", indoorOutdoor: "indoor", avgDuration: "60",
                extraMetadata: m => m
                    .With("number_of_slides", "4")
                    .With("min_height_solo_cm", "120")
                    .With("family_slide", "true")
                    .With("kamikaze_slide", "true")
                    .With("included_in_aquapark_ticket", "true")),

            // --- Brodzik dla Dzieci (Aqua Kids) ---
            BuildAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0004-4000-8000-000000000004"),
                "Aqua Kids — Strefa dla Dzieci",
                "Brodzik, fontanny, zjeżdżalnie i atrakcje wodne dla najmłodszych dzieci (do 120 cm wzrostu).",
                zone: "aqua_park", indoorOutdoor: "indoor", avgDuration: "90",
                extraMetadata: m => m
                    .With("max_height_cm", "120")
                    .With("water_depth_cm", "30")
                    .With("supervised", "true")
                    .With("price_1h_dzieciecy_pln", "14")
                    .With("included_in_aquapark_ticket", "true")),

            // --- Jacuzzi i Rzeka Leniwca ---
            BuildAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0005-4000-8000-000000000005"),
                "Rzeka Leniwca i Jacuzzi",
                "Powolna rzeka leniwca do relaksacyjnego unoszenia się na wodzie oraz zewnętrzne jacuzzi z hydromasażem.",
                zone: "aqua_park", indoorOutdoor: "indoor", avgDuration: "60",
                extraMetadata: m => m
                    .With("lazy_river_length_m", "60")
                    .With("jacuzzi_capacity", "12")
                    .With("water_temp_c", "32")
                    .With("included_in_aquapark_ticket", "true")),

            // ═══════════════════════════════════════════════
            // STREFA SPA & WELLNESS (16+)
            // ═══════════════════════════════════════════════

            // --- Strefa Saun ---
            BuildAdultAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0006-4000-8000-000000000006"),
                "Strefa Saun",
                "Kompleks saunowy z sauną fińską, sauną parową (hamam), sauną solną i sauną infrared. Wyłącznie dla gości 16+.",
                zone: "spa", indoorOutdoor: "indoor", avgDuration: "120",
                extraMetadata: m => m
                    .With("min_age", "16")
                    .With("sauna_finnish", "true")
                    .With("sauna_steam", "true")
                    .With("sauna_salt", "true")
                    .With("sauna_infrared", "true")
                    .With("price_normalny_pln", "45")
                    .With("price_combined_aqua_spa_pln", "75")),

            // --- Basen Zewnętrzny (sezon letni) ---
            BuildAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0007-4000-8000-000000000007"),
                "Basen Zewnętrzny",
                "Odkryty basen rekreacyjny dostępny sezonowo (maj–sierpień) z podgrzewaną wodą i leżakami.",
                zone: "outdoor", indoorOutdoor: "outdoor", avgDuration: "180",
                extraMetadata: m => m
                    .With("seasonal", "true")
                    .With("season_start", "1 V")
                    .With("season_end", "31 VIII")
                    .With("water_temp_c", "28")
                    .With("sun_loungers", "true")
                    .With("price_1h_normalny_pln", "22")
                    .With("price_1h_ulgowy_pln", "17")),

            // --- Masaż Klasyczny ---
            BuildAdultAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0008-4000-8000-000000000008"),
                "Masaż Klasyczny (50 min)",
                "Zabieg masażu klasycznego całego ciała wykonywany przez licencjonowanego fizjoterapeutę. Wymagana rezerwacja 24h wcześniej.",
                zone: "spa", indoorOutdoor: "indoor", avgDuration: "60",
                extraMetadata: m => m
                    .With("duration_min", "50")
                    .With("advance_booking_h", "24")
                    .With("price_pln", "150")
                    .With("licensed_therapist", "true")),

            // --- Masaż Gorącymi Kamieniami ---
            BuildAdultAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0009-4000-8000-000000000009"),
                "Masaż Gorącymi Kamieniami (60 min)",
                "Luksusowy masaż bazaltowymi kamieniami rozgrzewającymi mięśnie. Wymagana rezerwacja.",
                zone: "spa", indoorOutdoor: "indoor", avgDuration: "70",
                extraMetadata: m => m
                    .With("duration_min", "60")
                    .With("advance_booking_h", "24")
                    .With("price_pln", "190")
                    .With("licensed_therapist", "true")),

            // --- Aqua Aerobik ---
            BuildAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0010-4000-8000-000000000010"),
                "Aqua Aerobik — zajęcia grupowe",
                "Grupowe zajęcia aqua aerobiku w basenie rekreacyjnym. Harmonogram zajęć dostępny na parkwodny.pl.",
                zone: "aqua_park", indoorOutdoor: "indoor", avgDuration: "55",
                extraMetadata: m => m
                    .With("class_duration_min", "45")
                    .With("min_participants", "5")
                    .With("max_participants", "20")
                    .With("advance_booking_required", "true")
                    .With("price_per_person_pln", "30")),

            // --- Nauka Pływania (dzieci 4–12 lat) ---
            BuildAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0011-4000-8000-000000000011"),
                "Nauka Pływania — Dzieci (4–12 lat)",
                "Indywidualne i grupowe lekcje pływania dla dzieci w wieku 4–12 lat prowadzone przez instruktorów z licencją PZP.",
                zone: "aqua_park", indoorOutdoor: "indoor", avgDuration: "45",
                extraMetadata: m => m
                    .With("min_age", "4")
                    .With("max_age", "12")
                    .With("lesson_duration_min", "30")
                    .With("group_size_max", "6")
                    .With("price_individual_pln", "80")
                    .With("price_group_pln", "40")
                    .With("advance_booking_required", "true")
                    .With("instructor_license", "PZP")),

            // --- Nauka Pływania (dorośli) ---
            BuildAdultAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0012-4000-8000-000000000012"),
                "Nauka Pływania — Dorośli",
                "Indywidualne lekcje pływania dla dorosłych prowadzone przez certyfikowanego instruktora.",
                zone: "aqua_park", indoorOutdoor: "indoor", avgDuration: "60",
                extraMetadata: m => m
                    .With("min_age", "16")
                    .With("lesson_duration_min", "45")
                    .With("price_individual_pln", "100")
                    .With("advance_booking_required", "true")
                    .With("instructor_license", "PZP")),

            // --- Wypożyczalnia Sprzętu ---
            BuildAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0013-4000-8000-000000000013"),
                "Wypożyczalnia Sprzętu Wodnego",
                "Wypożyczenie okularków, czepka, deski do pływania, makaronu lub kamizelki ratunkowej dla dzieci.",
                zone: "aqua_park", indoorOutdoor: "indoor", avgDuration: "0",
                extraMetadata: m => m
                    .With("goggles_pln", "5")
                    .With("cap_pln", "3")
                    .With("kickboard_pln", "5")
                    .With("noodle_pln", "4")
                    .With("life_vest_pln", "5")),

            // --- Szatnie i Przechowalnia ---
            BuildAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-0014-4000-8000-000000000014"),
                "Szatnia i Przechowalnia bagażu",
                "Szatnie z indywidualnymi szafkami (klucz na elektronicznym breloku wliczonym w cenę wejścia).",
                zone: "aqua_park", indoorOutdoor: "indoor", avgDuration: "0",
                extraMetadata: m => m
                    .With("locker_included_in_ticket", "true")
                    .With("extra_locker_deposit_pln", "20")),
        };

        foreach (var attraction in attractions)
        {
            repo.Save(attraction);
        }

        return attractions;
    }

    private static ProductType BuildAttraction(
        IProductIdentifier id,
        string name,
        string description,
        string zone,
        string indoorOutdoor,
        string avgDuration,
        Func<ProductMetadata, ProductMetadata>? extraMetadata = null)
    {
        return BuildCore(id, name, description, zone, indoorOutdoor, avgDuration,
            StandardAvailability, extraMetadata);
    }

    private static ProductType BuildAdultAttraction(
        IProductIdentifier id,
        string name,
        string description,
        string zone,
        string indoorOutdoor,
        string avgDuration,
        Func<ProductMetadata, ProductMetadata>? extraMetadata = null)
    {
        return BuildCore(id, name, description, zone, indoorOutdoor, avgDuration,
            AdultOnly, extraMetadata);
    }

    private static ProductType BuildCore(
        IProductIdentifier id,
        string name,
        string description,
        string zone,
        string indoorOutdoor,
        string avgDuration,
        IApplicabilityConstraint applicability,
        Func<ProductMetadata, ProductMetadata>? extraMetadata)
    {
        var metadata = SeedHelpers.ParkWodnyBase()
            .With("zone", zone)
            .With("indoor_outdoor", indoorOutdoor)
            .With("avg_duration_min", avgDuration);

        if (extraMetadata != null)
            metadata = extraMetadata(metadata);

        return new ProductBuilder(
                id,
                ProductName.Of(name),
                ProductDescription.Of(description))
            .WithMetadata(metadata)
            .WithApplicabilityConstraint(applicability)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.IndividuallyTracked)
                .WithMandatoryFeature(TicketType)
                .WithMandatoryFeature(VisitDate)
                .WithMandatoryFeature(VisitTimeSlot)
                .WithOptionalFeature(DurationHours)
                .Build();
    }
}
