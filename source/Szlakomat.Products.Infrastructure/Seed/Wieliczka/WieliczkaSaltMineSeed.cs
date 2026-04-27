using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Quantity;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Infrastructure.Seed.Wieliczka;

/// <summary>
/// Seeds Kopalnia Soli "Wieliczka" offers based on the 2026 public offer:
/// tourist route, miners' route, graduation tower, school/family/special variants,
/// timed-entry schedules, prices, participation constraints, and combined packages.
/// </summary>
internal static class WieliczkaSaltMineSeed
{
    private static readonly DateOnly OfferStart = new(2026, 1, 1);
    private static readonly DateOnly OfferEnd = new(2026, 12, 31);
    private static readonly DateOnly HighSeasonStart = new(2026, 5, 1);
    private static readonly DateOnly HighSeasonEnd = new(2026, 9, 30);

    private static readonly ProductFeatureType VisitDate =
        ProductFeatureType.WithDateRange("visit_date", "2026-01-01", "2026-12-31");

    private static readonly string[] TouristRoutePlTimes =
    {
        "07:30", "07:45", "07:50", "07:55", "08:05", "08:20", "08:50", "09:20", "09:50",
        "10:20", "10:50", "11:20", "11:50", "12:20", "12:50", "13:20", "13:50", "14:20",
        "14:50", "15:20", "15:50", "16:20", "16:50", "17:00", "17:20", "18:00", "18:05",
        "18:20", "18:30"
    };

    private static readonly string[] TouristRouteForeignTimes =
    {
        "07:30", "08:00", "08:30", "09:00", "09:30", "10:00", "10:30", "10:45", "11:00",
        "11:30", "12:00", "12:30", "13:00", "13:30", "13:45", "14:00", "14:30", "15:00",
        "15:30", "16:00", "16:30", "16:45", "17:00", "17:30", "18:00", "18:30"
    };

    private static readonly string[] MinersRoutePlTimes =
    {
        "10:00", "13:00", "14:00", "16:00"
    };

    private static readonly string[] MinersRouteEnTimes =
    {
        "10:15", "13:15", "14:15", "16:15"
    };

    private static readonly string[] GraduationTowerTimes =
    {
        "09:00", "10:00", "11:00", "12:00", "13:00", "14:00",
        "15:00", "16:00", "17:00", "18:00", "19:00"
    };

    public static void Seed(
        IProductTypeRepository productRepo,
        IPackageTypeRepository packageRepo,
        ICatalogEntryRepository catalogRepo,
        IProductRelationshipRepository relationshipRepo,
        ProductRelationshipFactory relationshipFactory)
    {
        var offers = SeedOffers(productRepo);
        var packages = SeedPackages(packageRepo, offers);

        SeedCatalog(catalogRepo, offers, packages);
        SeedRelationships(relationshipRepo, relationshipFactory, offers, packages);
    }

    private static List<ProductType> SeedOffers(IProductTypeRepository repo)
    {
        var offers = new List<ProductType>
        {
            BuildTouristRouteIndividualPl(),
            BuildTouristRouteIndividualForeign(),
            BuildTouristRouteAccessible(),
            BuildTouristRouteGroupPl(),
            BuildTouristRouteGroupForeign(),
            BuildTouristRouteSchool(),
            BuildMinersRouteIndividualPl(),
            BuildMinersRouteIndividualEn(),
            BuildMinersRouteGroupPl(),
            BuildMinersRouteGroupForeign(),
            BuildMinersRouteSchool(),
            BuildPilgrimTrailGroup(),
            BuildGraduationTower(),
            BuildSolilandiaSchool(),
            BuildSladamiLegendFamily(),
        };

        foreach (var offer in offers)
        {
            repo.Save(offer);
        }

        return offers;
    }

    private static ProductType BuildTouristRouteIndividualPl()
    {
        var metadata = TouristRouteBase()
            .With("offer_variant", "individual_pl")
            .With("start_location", "Szyb Daniłowicza")
            .With("start_address", "ul. Daniłowicza 10, Wieliczka")
            .With("end_location", "Szyb Regis / Szyb Daniłowicza")
            .With("guide_required", "true")
            .With("languages", "PL")
            .With("schedule.low.weekday.pl", "09:50-15:50 every 60 min; 17:00")
            .With("schedule.low.weekend.pl", "09:20-16:20 every 30 min; 17:00")
            .With("schedule.mid.weekday.pl", "08:50-16:50 every 60 min; 18:00")
            .With("schedule.mid.weekend.pl", "08:20-16:50 every 30 min; 18:00")
            .With("schedule.summer.pl", "08:05-18:05 every 15 min; 18:30")
            .With("price_2026_low_normal_pln", "103")
            .With("price_2026_high_normal_pln", "119")
            .With("price_2026_low_reduced_pln", "82")
            .With("price_2026_high_reduced_pln", "93")
            .With("price_2026_low_family_2_1_pln", "255")
            .With("price_2026_high_family_2_1_pln", "294")
            .With("price_2026_low_family_2_2_pln", "304")
            .With("price_2026_high_family_2_2_pln", "350")
            .With("price_2026_low_kdr_normal_pln", "72")
            .With("price_2026_high_kdr_normal_pln", "83")
            .With("price_2026_low_kdr_reduced_pln", "57")
            .With("price_2026_high_kdr_reduced_pln", "65")
            .With("free_child_under_age", "4")
            .With("summer_family_bonus", "free_graduation_tower_entry_july_august_2026_same_day");

        metadata = WithCommonSafetyRules(metadata, minAge: "none", maxGroupSize: "35");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111111"),
            "Kopalnia Soli Wieliczka - Trasa Turystyczna - indywidualnie PL",
            "Indywidualne zwiedzanie Trasy Turystycznej w języku polskim z wejściem na konkretną godzinę.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 1, 35),
            new[] { "PL" },
            new[] { "normal", "reduced", "disabled_reduced", "family_2_1", "family_2_2", "kdr_normal", "kdr_reduced", "child_under_4_control" },
            TouristRoutePlTimes,
            "individual",
            (1, 35),
            (0, 120));
    }

    private static ProductType BuildTouristRouteIndividualForeign()
    {
        var metadata = TouristRouteBase()
            .With("offer_variant", "individual_foreign")
            .With("start_location", "Szyb Daniłowicza")
            .With("start_address", "ul. Daniłowicza 10, Wieliczka")
            .With("end_location", "Szyb Regis / Szyb Daniłowicza")
            .With("guide_required", "true")
            .With("languages", "EN,DE,FR,IT,ES,RU,UA")
            .With("schedule.en.low.weekday", "09:00-17:00 every 60 min")
            .With("schedule.en.low.weekend", "09:00-17:00 every 30 min")
            .With("schedule.en.mid", "08:30-18:00 every 30 min")
            .With("schedule.en.summer", "07:30-18:30 every 30 min")
            .With("schedule.ru.low", "13:45")
            .With("schedule.ru.summer", "10:45, 13:45, 16:45")
            .With("price_2026_low_normal_pln", "143")
            .With("price_2026_high_normal_pln", "159")
            .With("price_2026_low_reduced_pln", "121")
            .With("price_2026_high_reduced_pln", "131")
            .With("price_2026_low_family_2_1_pln", "358")
            .With("price_2026_high_family_2_1_pln", "397")
            .With("price_2026_low_family_2_2_pln", "430")
            .With("price_2026_high_family_2_2_pln", "475")
            .With("online_purchase_required_for_non_pl_en", "true")
            .With("free_child_under_age", "4");

        metadata = WithCommonSafetyRules(metadata, minAge: "none", maxGroupSize: "35");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111112"),
            "Kopalnia Soli Wieliczka - Trasa Turystyczna - indywidualnie język obcy",
            "Indywidualne zwiedzanie Trasy Turystycznej w wybranych językach obcych.",
            metadata,
            ApplicabilityConstraint.And(
                ApplicabilityConstraint.Between("group_size", 1, 35),
                ApplicabilityConstraint.In("language", "EN", "DE", "FR", "IT", "ES", "RU", "UA")),
            new[] { "EN", "DE", "FR", "IT", "ES", "RU", "UA" },
            new[] { "normal", "reduced", "disabled_reduced", "family_2_1", "family_2_2", "child_under_4_control" },
            TouristRouteForeignTimes,
            "individual",
            (1, 35),
            (0, 120));
    }

    private static ProductType BuildTouristRouteAccessible()
    {
        var metadata = TouristRouteBase()
            .With("offer_variant", "accessible")
            .With("start_location", "Szyb Daniłowicza")
            .With("start_address", "ul. Daniłowicza 10, Wieliczka")
            .With("route_scope", "selected_level_II_workings")
            .With("languages", "PL,EN")
            .With("advance_reservation_required", "true")
            .With("reservation_email", "rezerwacja@kopalnia.pl")
            .With("available_hours_rule", "first_and_last_reception_hour")
            .With("available_hours_summer", "08:00 and last reception hour")
            .With("places_limited", "true")
            .With("assistant_dog_allowed", "true")
            .With("price_2026_low_reduced_pln", "82")
            .With("price_2026_high_reduced_pln", "93");

        metadata = WithRule(metadata, 1, "reservation", "advance_reservation", "equals", "required");
        metadata = WithRule(metadata, 2, "accessibility", "route_scope", "equals", "partial_only");
        metadata = WithRule(metadata, 3, "capacity", "places", "equals", "limited");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111113"),
            "Kopalnia Soli Wieliczka - Trasa Turystyczna - dostępna PL/EN",
            "Wariant dla osób z niepełnosprawnością ruchową, realizowany po wcześniejszej rezerwacji.",
            metadata,
            ApplicabilityConstraint.And(
                ApplicabilityConstraint.Between("group_size", 1, 8),
                ApplicabilityConstraint.In("language", "PL", "EN")),
            new[] { "PL", "EN" },
            new[] { "disabled_reduced" },
            new[] { "08:00", "09:00", "17:00", "18:00" },
            "accessibility",
            (1, 8),
            (0, 120));
    }

    private static ProductType BuildTouristRouteGroupPl()
    {
        var metadata = TouristRouteBase()
            .With("offer_variant", "organized_group_pl")
            .With("start_location", "Szyb Daniłowicza")
            .With("start_address", "ul. Daniłowicza 10, Wieliczka")
            .With("guide_required", "true")
            .With("languages", "PL")
            .With("reservation_recommended", "true")
            .With("minimum_paid_tickets", "25")
            .With("guide_control_ticket_pln", "0")
            .With("price_2026_low_normal_pln", "103")
            .With("price_2026_high_normal_pln", "119")
            .With("price_2026_low_reduced_pln", "82")
            .With("price_2026_high_reduced_pln", "93");

        metadata = WithCommonSafetyRules(metadata, minAge: "none", maxGroupSize: "40");
        metadata = WithRule(metadata, 20, "group", "paid_tickets", "min", "25");
        metadata = WithRule(metadata, 21, "group", "guide_control_ticket", "max", "1");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111114"),
            "Kopalnia Soli Wieliczka - Trasa Turystyczna - grupa PL",
            "Grupowe zwiedzanie Trasy Turystycznej w języku polskim.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 25, 40),
            new[] { "PL" },
            new[] { "normal", "reduced", "disabled_reduced", "guide_control", "child_under_4_control" },
            TouristRoutePlTimes,
            "organized_group",
            (25, 40),
            (0, 120));
    }

    private static ProductType BuildTouristRouteGroupForeign()
    {
        var metadata = TouristRouteBase()
            .With("offer_variant", "organized_group_foreign")
            .With("start_location", "Szyb Daniłowicza")
            .With("start_address", "ul. Daniłowicza 10, Wieliczka")
            .With("guide_required", "true")
            .With("languages_standard", "EN,FR,DE,IT,ES,RU")
            .With("languages_on_request", "HR,JA,UA")
            .With("reservation_required_for_other_languages", "true")
            .With("minimum_paid_tickets", "25")
            .With("guide_control_ticket_pln", "0")
            .With("price_2026_low_normal_pln", "143")
            .With("price_2026_high_normal_pln", "159")
            .With("price_2026_low_reduced_pln", "121")
            .With("price_2026_high_reduced_pln", "131");

        metadata = WithCommonSafetyRules(metadata, minAge: "none", maxGroupSize: "40");
        metadata = WithRule(metadata, 20, "group", "paid_tickets", "min", "25");
        metadata = WithRule(metadata, 21, "language", "non_standard_language", "requires", "operator_availability");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111115"),
            "Kopalnia Soli Wieliczka - Trasa Turystyczna - grupa język obcy",
            "Grupowe zwiedzanie Trasy Turystycznej w językach obcych, z możliwością zapytania o języki niestandardowe.",
            metadata,
            ApplicabilityConstraint.And(
                ApplicabilityConstraint.Between("group_size", 25, 40),
                ApplicabilityConstraint.In("language", "EN", "FR", "DE", "IT", "ES", "RU", "HR", "JA", "UA")),
            new[] { "EN", "FR", "DE", "IT", "ES", "RU", "HR", "JA", "UA" },
            new[] { "normal", "reduced", "disabled_reduced", "guide_control", "child_under_4_control" },
            TouristRouteForeignTimes,
            "organized_group",
            (25, 40),
            (0, 120));
    }

    private static ProductType BuildTouristRouteSchool()
    {
        var metadata = TouristRouteBase()
            .With("offer_variant", "school_pl")
            .With("start_location", "Szyb Daniłowicza")
            .With("start_address", "ul. Daniłowicza 10, Wieliczka")
            .With("languages", "PL")
            .With("minimum_school_tickets", "20")
            .With("teacher_control_ticket_rule", "1 per each started 10 paid school tickets")
            .With("price_2026_low_school_pln", "48")
            .With("price_2026_high_school_pln", "48")
            .With("school_offer_source", "group_school_sales_rules");

        metadata = WithCommonSafetyRules(metadata, minAge: "none", maxGroupSize: "40");
        metadata = WithRule(metadata, 20, "school", "paid_school_tickets", "min", "20");
        metadata = WithRule(metadata, 21, "school", "teacher_control_ticket", "ratio", "1_per_started_10");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111116"),
            "Kopalnia Soli Wieliczka - Trasa Turystyczna - szkoły PL",
            "Szkolny wariant Trasy Turystycznej w języku polskim.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 20, 40),
            new[] { "PL" },
            new[] { "school", "teacher_control" },
            TouristRoutePlTimes,
            "school",
            (20, 40),
            (0, 120));
    }

    private static ProductType BuildMinersRouteIndividualPl()
    {
        var metadata = MinersRouteBase()
            .With("offer_variant", "individual_pl")
            .With("start_location", "Szyb Regis")
            .With("start_address", "pl. Kościuszki 9, Wieliczka")
            .With("end_location", "Szyb Regis")
            .With("guide_required", "true")
            .With("guide_role", "przodowy")
            .With("languages", "PL")
            .With("schedule.low.pl", "10:00, 14:00")
            .With("schedule.high.pl", "10:00, 13:00, 16:00")
            .With("arrival_before_entry_min", "15")
            .With("price_2026_low_normal_pln", "103")
            .With("price_2026_high_normal_pln", "119")
            .With("price_2026_low_reduced_pln", "82")
            .With("price_2026_high_reduced_pln", "93")
            .With("price_2026_low_family_2_1_pln", "255")
            .With("price_2026_high_family_2_1_pln", "294")
            .With("price_2026_low_family_2_2_pln", "304")
            .With("price_2026_high_family_2_2_pln", "350")
            .With("summer_family_bonus", "free_graduation_tower_entry_july_august_2026_same_day");

        metadata = WithMinersSafetyRules(metadata, maxGroupSize: "20");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111117"),
            "Kopalnia Soli Wieliczka - Trasa Górnicza - indywidualnie PL",
            "Aktywna wyprawa Trasą Górniczą w języku polskim, z górniczym szkoleniem i wyposażeniem.",
            metadata,
            ApplicabilityConstraint.And(
                ApplicabilityConstraint.Between("group_size", 1, 20),
                ApplicabilityConstraint.Between("visitor_age", 10, 120)),
            new[] { "PL" },
            new[] { "normal", "reduced", "family_2_1", "family_2_2", "kdr_normal", "kdr_reduced" },
            MinersRoutePlTimes,
            "individual",
            (1, 20),
            (10, 120));
    }

    private static ProductType BuildMinersRouteIndividualEn()
    {
        var metadata = MinersRouteBase()
            .With("offer_variant", "individual_en")
            .With("start_location", "Szyb Regis")
            .With("start_address", "pl. Kościuszki 9, Wieliczka")
            .With("end_location", "Szyb Regis")
            .With("guide_required", "true")
            .With("guide_role", "przodowy")
            .With("languages", "EN")
            .With("schedule.low.en", "10:15, 14:15")
            .With("schedule.high.en", "10:15, 13:15, 16:15")
            .With("arrival_before_entry_min", "15")
            .With("price_2026_low_normal_pln", "143")
            .With("price_2026_high_normal_pln", "159")
            .With("price_2026_low_reduced_pln", "121")
            .With("price_2026_high_reduced_pln", "131")
            .With("price_2026_low_family_2_1_pln", "358")
            .With("price_2026_high_family_2_1_pln", "397")
            .With("price_2026_low_family_2_2_pln", "430")
            .With("price_2026_high_family_2_2_pln", "475");

        metadata = WithMinersSafetyRules(metadata, maxGroupSize: "20");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111118"),
            "Kopalnia Soli Wieliczka - Trasa Górnicza - indywidualnie EN",
            "Aktywna wyprawa Trasą Górniczą w języku angielskim.",
            metadata,
            ApplicabilityConstraint.And(
                ApplicabilityConstraint.Between("group_size", 1, 20),
                ApplicabilityConstraint.Between("visitor_age", 10, 120),
                ApplicabilityConstraint.In("language", "EN")),
            new[] { "EN" },
            new[] { "normal", "reduced", "family_2_1", "family_2_2" },
            MinersRouteEnTimes,
            "individual",
            (1, 20),
            (10, 120));
    }

    private static ProductType BuildMinersRouteGroupPl()
    {
        var metadata = MinersRouteBase()
            .With("offer_variant", "organized_group_pl")
            .With("start_location", "Szyb Regis")
            .With("start_address", "pl. Kościuszki 9, Wieliczka")
            .With("languages", "PL")
            .With("minimum_paid_tickets", "10")
            .With("guide_control_ticket_pln", "0")
            .With("price_2026_low_normal_pln", "103")
            .With("price_2026_high_normal_pln", "119")
            .With("price_2026_low_reduced_pln", "82")
            .With("price_2026_high_reduced_pln", "93");

        metadata = WithMinersSafetyRules(metadata, maxGroupSize: "20");
        metadata = WithRule(metadata, 20, "group", "paid_tickets", "min", "10");
        metadata = WithRule(metadata, 21, "group", "guide_control_ticket", "max", "1");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111119"),
            "Kopalnia Soli Wieliczka - Trasa Górnicza - grupa PL",
            "Grupowe zwiedzanie Trasy Górniczej po polsku.",
            metadata,
            ApplicabilityConstraint.And(
                ApplicabilityConstraint.Between("group_size", 10, 20),
                ApplicabilityConstraint.Between("visitor_age", 10, 120)),
            new[] { "PL" },
            new[] { "normal", "reduced", "guide_control" },
            MinersRoutePlTimes,
            "organized_group",
            (10, 20),
            (10, 120));
    }

    private static ProductType BuildMinersRouteGroupForeign()
    {
        var metadata = MinersRouteBase()
            .With("offer_variant", "organized_group_foreign")
            .With("start_location", "Szyb Regis")
            .With("start_address", "pl. Kościuszki 9, Wieliczka")
            .With("languages", "EN,DE,FR,IT,ES,RU,UA")
            .With("minimum_paid_tickets", "10")
            .With("guide_control_ticket_pln", "0")
            .With("price_2026_low_normal_pln", "143")
            .With("price_2026_high_normal_pln", "159")
            .With("price_2026_low_reduced_pln", "121")
            .With("price_2026_high_reduced_pln", "131")
            .With("non_standard_language_requires_operator_availability", "true");

        metadata = WithMinersSafetyRules(metadata, maxGroupSize: "20");
        metadata = WithRule(metadata, 20, "group", "paid_tickets", "min", "10");
        metadata = WithRule(metadata, 21, "language", "non_standard_language", "requires", "operator_availability");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111120"),
            "Kopalnia Soli Wieliczka - Trasa Górnicza - grupa język obcy",
            "Grupowe zwiedzanie Trasy Górniczej w językach obcych.",
            metadata,
            ApplicabilityConstraint.And(
                ApplicabilityConstraint.Between("group_size", 10, 20),
                ApplicabilityConstraint.Between("visitor_age", 10, 120),
                ApplicabilityConstraint.In("language", "EN", "DE", "FR", "IT", "ES", "RU", "UA")),
            new[] { "EN", "DE", "FR", "IT", "ES", "RU", "UA" },
            new[] { "normal", "reduced", "guide_control" },
            MinersRouteEnTimes,
            "organized_group",
            (10, 20),
            (10, 120));
    }

    private static ProductType BuildMinersRouteSchool()
    {
        var metadata = MinersRouteBase()
            .With("offer_variant", "school_pl")
            .With("start_location", "Szyb Regis")
            .With("start_address", "pl. Kościuszki 9, Wieliczka")
            .With("languages", "PL")
            .With("minimum_school_tickets", "10")
            .With("maximum_group_size", "20")
            .With("school_language_limit", "PL_only")
            .With("online_purchase_required", "true")
            .With("teacher_control_ticket_rule", "1 per each started 10 paid school tickets")
            .With("price_2026_low_school_pln", "48")
            .With("price_2026_high_school_pln", "48");

        metadata = WithMinersSafetyRules(metadata, maxGroupSize: "20");
        metadata = WithRule(metadata, 20, "school", "paid_school_tickets", "min", "10");
        metadata = WithRule(metadata, 21, "school", "group_size", "max", "20");
        metadata = WithRule(metadata, 22, "school", "language", "equals", "PL");
        metadata = WithRule(metadata, 23, "school", "teacher_control_ticket", "ratio", "1_per_started_10");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111121"),
            "Kopalnia Soli Wieliczka - Trasa Górnicza - szkoły PL",
            "Szkolny wariant Trasy Górniczej, tylko po polsku, dla uczniów od 10 lat.",
            metadata,
            ApplicabilityConstraint.And(
                ApplicabilityConstraint.Between("group_size", 10, 20),
                ApplicabilityConstraint.Between("visitor_age", 10, 120),
                ApplicabilityConstraint.In("language", "PL")),
            new[] { "PL" },
            new[] { "school", "teacher_control" },
            MinersRoutePlTimes,
            "school",
            (10, 20),
            (10, 120));
    }

    private static ProductType BuildPilgrimTrailGroup()
    {
        var metadata = TouristRouteBase()
            .With("route_code", "pilgrim_trail")
            .With("route_name", "Szlak Pielgrzymkowy")
            .With("offer_variant", "organized_group")
            .With("start_location", "Szyb Daniłowicza")
            .With("start_address", "ul. Daniłowicza 10, Wieliczka")
            .With("duration_hours", "2.5")
            .With("distance_km", "2.5")
            .With("temperature_c", "17-18")
            .With("max_depth_m", "135")
            .With("guide_required", "true")
            .With("mass_possible", "true")
            .With("schedule.group", "08:00-18:00 by reservation")
            .With("price_from_pln", "102");

        metadata = WithCommonSafetyRules(metadata, minAge: "none", maxGroupSize: "40");
        metadata = WithRule(metadata, 20, "group", "reservation", "equals", "required");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111122"),
            "Kopalnia Soli Wieliczka - Szlak Pielgrzymkowy - grupa",
            "Grupowy szlak religijny z możliwością mszy w podziemnej kaplicy.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 10, 40),
            new[] { "PL" },
            new[] { "normal", "reduced", "guide_control" },
            TouristRoutePlTimes,
            "pilgrim",
            (10, 40),
            (0, 120));
    }

    private static ProductType BuildGraduationTower()
    {
        var metadata = WieliczkaBase()
            .With("route_code", "graduation_tower")
            .With("route_name", "Tężnia Solankowa")
            .With("offer_variant", "individual_entry")
            .With("product_kind", "surface_attraction")
            .With("location", "Park św. Kingi")
            .With("guided", "false")
            .With("opening_time", "09:00")
            .With("last_entry_time", "19:00")
            .With("recommended_visit_min", "30")
            .With("tower_height_m", "22.5")
            .With("area_m2", "3200")
            .With("price_2026_entry_pln", "10")
            .With("price_2026_kdr_pln", "7")
            .With("price_2026_resident_pln", "2")
            .With("price_2026_resident_5_entry_pass_pln", "10")
            .With("price_2026_5_entry_pass_pln", "40")
            .With("pass_validity_days", "30")
            .With("additional_fee_no_valid_ticket_pln", "50")
            .With("no_age_limit", "true");

        metadata = WithRule(metadata, 1, "entry", "last_entry_time", "max", "19:00");
        metadata = WithRule(metadata, 2, "pass", "validity_days", "equals", "30");
        metadata = WithRule(metadata, 3, "penalty", "missing_valid_ticket", "amount_pln", "50");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111123"),
            "Kopalnia Soli Wieliczka - Tężnia Solankowa - wejście",
            "Samodzielny pobyt w Tężni Solankowej, także jako dodatek do wybranych tras.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 1, 200),
            new[] { "NONE" },
            new[] { "normal", "kdr_normal", "wieliczka_resident", "pass_5_entries", "child_under_4_control", "additional_fee" },
            GraduationTowerTimes,
            "individual",
            (1, 200),
            (0, 120));
    }

    private static ProductType BuildSolilandiaSchool()
    {
        var metadata = TouristRouteBase()
            .With("route_code", "solilandia")
            .With("route_name", "Odkrywamy Solilandię")
            .With("offer_variant", "school_special")
            .With("start_location", "Szyb Daniłowicza")
            .With("languages", "PL")
            .With("target_group", "preschool_and_primary_school")
            .With("minimum_paid_tickets", "25")
            .With("price_2026_low_school_pln", "75")
            .With("price_2026_high_school_pln", "75")
            .With("price_2026_low_adult_pln", "103")
            .With("price_2026_high_adult_pln", "119")
            .With("teacher_control_ticket_rule", "1 per each started 10 paid school tickets");

        metadata = WithCommonSafetyRules(metadata, minAge: "none", maxGroupSize: "40");
        metadata = WithRule(metadata, 20, "school", "paid_school_tickets", "min", "25");
        metadata = WithRule(metadata, 21, "school", "teacher_control_ticket", "ratio", "1_per_started_10");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111124"),
            "Kopalnia Soli Wieliczka - Odkrywamy Solilandię - szkoły",
            "Tematyczna oferta szkolna dla dzieci przedszkolnych i szkół podstawowych.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 25, 40),
            new[] { "PL" },
            new[] { "school", "normal", "teacher_control" },
            TouristRoutePlTimes,
            "school",
            (25, 40),
            (0, 15));
    }

    private static ProductType BuildSladamiLegendFamily()
    {
        var metadata = TouristRouteBase()
            .With("route_code", "sladami_legend")
            .With("route_name", "Śladami Legend")
            .With("offer_variant", "family_special")
            .With("start_location", "Szyb Daniłowicza")
            .With("languages", "PL")
            .With("target_group", "families_with_children_5_12")
            .With("special_attractions", "meeting_with_skarbnik")
            .With("online_tickets_available", "true")
            .With("price_from_pln", "105");

        metadata = WithCommonSafetyRules(metadata, minAge: "5", maxGroupSize: "35");
        metadata = WithRule(metadata, 20, "family", "child_age", "between", "5-12");
        metadata = WithRule(metadata, 21, "family", "adult_guardian", "equals", "required");

        return BuildOffer(
            UuidProductIdentifier.Of("11111111-1111-4111-8111-111111111125"),
            "Kopalnia Soli Wieliczka - Śladami Legend - rodziny",
            "Rodzinna wyprawa specjalna dla dzieci w wieku 5-12 lat, z atrakcjami dla najmłodszych.",
            metadata,
            ApplicabilityConstraint.And(
                ApplicabilityConstraint.Between("group_size", 2, 35),
                ApplicabilityConstraint.Between("visitor_age", 5, 120)),
            new[] { "PL" },
            new[] { "normal", "reduced", "family_2_1", "family_2_2" },
            TouristRoutePlTimes,
            "family",
            (2, 35),
            (5, 120));
    }

    private static List<PackageType> SeedPackages(IPackageTypeRepository repo, List<ProductType> offers)
    {
        var byName = offers.ToDictionary(o => o.Name().Value);
        var touristPl = byName["Kopalnia Soli Wieliczka - Trasa Turystyczna - indywidualnie PL"];
        var touristForeign = byName["Kopalnia Soli Wieliczka - Trasa Turystyczna - indywidualnie język obcy"];
        var minersPl = byName["Kopalnia Soli Wieliczka - Trasa Górnicza - indywidualnie PL"];
        var minersEn = byName["Kopalnia Soli Wieliczka - Trasa Górnicza - indywidualnie EN"];
        var tower = byName["Kopalnia Soli Wieliczka - Tężnia Solankowa - wejście"];
        var legends = byName["Kopalnia Soli Wieliczka - Śladami Legend - rodziny"];

        var packages = new List<PackageType>
        {
            BuildPackage(
                UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222221"),
                "Kopalnia Soli Wieliczka - Trasa Turystyczna + Tężnia - PL",
                "Pakiet: Trasa Turystyczna po polsku oraz pobyt w Tężni Solankowej w dniu zwiedzania.",
                WieliczkaBase()
                    .With("package_type", "route_plus_graduation_tower")
                    .With("route_time_binding", "same_day")
                    .With("price_2026_low_normal_pln", "109")
                    .With("price_2026_high_normal_pln", "126")
                    .With("price_2026_low_reduced_pln", "88")
                    .With("price_2026_high_reduced_pln", "100"),
                ProductTrackingStrategy.IndividuallyTracked,
                b => b.WithSingleChoice("route", touristPl.Id()).WithSingleChoice("graduation_tower", tower.Id())),

            BuildPackage(
                UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222222"),
                "Kopalnia Soli Wieliczka - Trasa Górnicza + Tężnia - PL",
                "Pakiet: Trasa Górnicza po polsku oraz pobyt w Tężni Solankowej w dniu zwiedzania.",
                WieliczkaBase()
                    .With("package_type", "route_plus_graduation_tower")
                    .With("route_time_binding", "same_day")
                    .With("price_2026_low_normal_pln", "109")
                    .With("price_2026_high_normal_pln", "126")
                    .With("price_2026_low_reduced_pln", "88")
                    .With("price_2026_high_reduced_pln", "100"),
                ProductTrackingStrategy.IndividuallyTracked,
                b => b.WithSingleChoice("route", minersPl.Id()).WithSingleChoice("graduation_tower", tower.Id())),

            BuildPackage(
                UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222223"),
                "Kopalnia Soli Wieliczka - Dwie trasy EN",
                "Pakiet testujący kompozycję: Trasa Turystyczna i Trasa Górnicza w języku angielskim.",
                WieliczkaBase()
                    .With("package_type", "two_routes")
                    .With("language", "EN")
                    .With("requires_two_separate_entry_slots", "true")
                    .With("same_day_possible", "depends_on_slot_availability"),
                ProductTrackingStrategy.IndividuallyTracked,
                b => b.WithSingleChoice("tourist_route", touristForeign.Id()).WithSingleChoice("miners_route", minersEn.Id())),

            BuildPackage(
                UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222224"),
                "Kopalnia Soli Wieliczka - Rodzinny dzień w kopalni",
                "Pakiet rodzinny: wybór Trasy Turystycznej lub Śladami Legend oraz opcjonalny pobyt w tężni.",
                WieliczkaBase()
                    .With("package_type", "family_day")
                    .With("family_age_rules_differ_by_route", "true"),
                ProductTrackingStrategy.BatchTracked,
                b => b.WithSingleChoice("family_route", touristPl.Id(), legends.Id()).WithOptionalChoice("graduation_tower", tower.Id()))
        };

        foreach (var package in packages)
        {
            repo.Save(package);
        }

        return packages;
    }

    private static void SeedCatalog(ICatalogEntryRepository repo, List<ProductType> offers, List<PackageType> packages)
    {
        var entries = new List<CatalogEntry>();

        foreach (var offer in offers)
        {
            entries.Add(BuildEntry(
                offer,
                offer.Name().Value,
                offer.Description().Value,
                CategoriesFor(offer),
                Validity.Between(OfferStart, OfferEnd),
                ("source", "kopalnia.pl"),
                ("season_high_from", HighSeasonStart.ToString("yyyy-MM-dd")),
                ("season_high_to", HighSeasonEnd.ToString("yyyy-MM-dd"))));
        }

        foreach (var package in packages)
        {
            entries.Add(BuildEntry(
                package,
                package.Name().Value,
                package.Description().Value,
                new[] { "wieliczka", "kopalnia-soli", "pakiet" },
                Validity.Between(OfferStart, OfferEnd),
                ("source", "kopalnia.pl"),
                ("composition", "package_type")));
        }

        foreach (var entry in entries)
        {
            repo.Save(entry);
        }
    }

    private static void SeedRelationships(
        IProductRelationshipRepository repo,
        ProductRelationshipFactory factory,
        List<ProductType> offers,
        List<PackageType> packages)
    {
        var offer = offers.ToDictionary(o => o.Name().Value);
        var package = packages.ToDictionary(p => p.Name().Value);
        var relationships = new List<ProductRelationship>();

        Define(factory, relationships,
            offer["Kopalnia Soli Wieliczka - Trasa Turystyczna - indywidualnie PL"],
            package["Kopalnia Soli Wieliczka - Trasa Turystyczna + Tężnia - PL"],
            ProductRelationshipType.UpgradableTo);
        Define(factory, relationships,
            offer["Kopalnia Soli Wieliczka - Trasa Górnicza - indywidualnie PL"],
            package["Kopalnia Soli Wieliczka - Trasa Górnicza + Tężnia - PL"],
            ProductRelationshipType.UpgradableTo);
        Define(factory, relationships,
            offer["Kopalnia Soli Wieliczka - Trasa Turystyczna - indywidualnie język obcy"],
            package["Kopalnia Soli Wieliczka - Dwie trasy EN"],
            ProductRelationshipType.ComplementedBy);
        Define(factory, relationships,
            offer["Kopalnia Soli Wieliczka - Trasa Górnicza - indywidualnie EN"],
            package["Kopalnia Soli Wieliczka - Dwie trasy EN"],
            ProductRelationshipType.ComplementedBy);
        Define(factory, relationships,
            offer["Kopalnia Soli Wieliczka - Trasa Turystyczna - indywidualnie PL"],
            offer["Kopalnia Soli Wieliczka - Śladami Legend - rodziny"],
            ProductRelationshipType.SubstitutedBy);
        Define(factory, relationships,
            offer["Kopalnia Soli Wieliczka - Trasa Turystyczna - indywidualnie PL"],
            offer["Kopalnia Soli Wieliczka - Tężnia Solankowa - wejście"],
            ProductRelationshipType.ComplementedBy);
        Define(factory, relationships,
            offer["Kopalnia Soli Wieliczka - Trasa Górnicza - indywidualnie PL"],
            offer["Kopalnia Soli Wieliczka - Tężnia Solankowa - wejście"],
            ProductRelationshipType.ComplementedBy);
        Define(factory, relationships,
            offer["Kopalnia Soli Wieliczka - Trasa Górnicza - indywidualnie PL"],
            offer["Kopalnia Soli Wieliczka - Trasa Turystyczna - dostępna PL/EN"],
            ProductRelationshipType.IncompatibleWith);

        foreach (var relationship in relationships)
        {
            repo.Save(relationship);
        }
    }

    private static ProductType BuildOffer(
        IProductIdentifier id,
        string name,
        string description,
        ProductMetadata metadata,
        IApplicabilityConstraint applicability,
        string[] languages,
        string[] ticketTypes,
        string[] entryTimes,
        string customerSegment,
        (int Min, int Max) groupSizeRange,
        (int Min, int Max) visitorAgeRange)
    {
        return new ProductBuilder(
                id,
                ProductName.Of(name),
                ProductDescription.Of(description))
            .WithMetadata(metadata)
            .WithApplicabilityConstraint(applicability)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.IndividuallyTracked)
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("ticket_type", ticketTypes))
                .WithMandatoryFeature(VisitDate)
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("entry_time", entryTimes))
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("language", languages))
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("customer_segment", customerSegment))
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("group_size", groupSizeRange.Min, groupSizeRange.Max))
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("visitor_age", visitorAgeRange.Min, visitorAgeRange.Max))
                .Build();
    }

    private static PackageType BuildPackage(
        IProductIdentifier id,
        string name,
        string description,
        ProductMetadata metadata,
        ProductTrackingStrategy trackingStrategy,
        Func<ProductBuilder.PackageTypeBuilder, ProductBuilder.PackageTypeBuilder> configure)
    {
        var builder = new ProductBuilder(
                id,
                ProductName.Of(name),
                ProductDescription.Of(description))
            .WithMetadata(metadata)
            .WithApplicabilityConstraint(ApplicabilityConstraint.AlwaysTrue())
            .AsPackageType()
                .WithTrackingStrategy(trackingStrategy);

        return configure(builder).Build();
    }

    private static CatalogEntry BuildEntry(
        IProduct product,
        string displayName,
        string description,
        string[] categories,
        Validity validity,
        params (string key, string value)[] metadata)
    {
        var builder = CatalogEntry.CreateBuilder()
            .WithId(CatalogEntryId.Generate())
            .WithDisplayName(displayName)
            .WithDescription(description)
            .WithProduct(product)
            .WithValidity(validity);

        foreach (var category in categories)
        {
            builder.WithCategory(category);
        }

        foreach (var (key, value) in metadata)
        {
            builder.WithMetadata(key, value);
        }

        foreach (var (key, value) in product.Metadata().AsMap())
        {
            builder.WithMetadata(key, value);
        }

        return builder.Build();
    }

    private static ProductMetadata WieliczkaBase() =>
        ProductMetadata.Empty()
            .With("city", "Wieliczka")
            .With("region", "Małopolska")
            .With("operator", "Kopalnia Soli Wieliczka")
            .With("category", "kopalnia-soli")
            .With("unesco", "true")
            .With("price_year", "2026")
            .With("season_low", "2026-01-01..2026-04-30;2026-10-01..2026-12-31")
            .With("season_high", "2026-05-01..2026-09-30")
            .With("closed_days", "2026-01-01;2026-04-05;2026-11-01;2026-12-24;2026-12-25")
            .With("domain_source", "kopalnia.pl;kopalniawieliczka.eu");

    private static ProductMetadata TouristRouteBase() =>
        WieliczkaBase()
            .With("route_code", "tourist_route")
            .With("route_name", "Trasa Turystyczna")
            .With("product_kind", "timed_guided_route")
            .With("duration_hours", "2-3")
            .With("distance_km", "3.5")
            .With("temperature_c", "17-18")
            .With("max_depth_m", "135")
            .With("stairs_total", "800")
            .With("stairs_at_start", "380")
            .With("museum_included", "Muzeum Żup Krakowskich Wieliczka")
            .With("small_luggage_max_cm", "20x20x35")
            .With("strollers_discouraged", "true")
            .With("phone_signal", "limited")
            .With("wifi", "selected_place");

    private static ProductMetadata MinersRouteBase() =>
        WieliczkaBase()
            .With("route_code", "miners_route")
            .With("route_name", "Trasa Górnicza")
            .With("product_kind", "timed_guided_active_route")
            .With("duration_hours", "2-3")
            .With("distance_km", "1.9")
            .With("temperature_c", "14-16")
            .With("max_depth_m", "101")
            .With("protective_clothing", "required")
            .With("full_shoes", "required")
            .With("safety_training", "required")
            .With("sign_entry_book", "required")
            .With("lockers", "surface_regis_shaft")
            .With("no_food_points_on_route", "true")
            .With("no_wifi_or_mobile_signal", "true");

    private static ProductMetadata WithCommonSafetyRules(ProductMetadata metadata, string minAge, string maxGroupSize)
    {
        metadata = WithRule(metadata, 1, "age", "minimum_age", "equals", minAge);
        metadata = WithRule(metadata, 2, "supervision", "under_18", "requires", "adult_guardian");
        metadata = WithRule(metadata, 3, "guide", "guide", "equals", "required");
        metadata = WithRule(metadata, 4, "capacity", "group_size", "max", maxGroupSize);
        metadata = WithRule(metadata, 5, "physical", "stairs_total", "equals", "800");
        metadata = WithRule(metadata, 6, "luggage", "max_dimensions_cm", "equals", "20x20x35");
        metadata = WithRule(metadata, 7, "prohibited", "open_fire", "equals", "true");
        metadata = WithRule(metadata, 8, "prohibited", "animals", "except", "assistance_dogs");
        metadata = WithRule(metadata, 9, "prohibited", "smoking", "equals", "true");
        return metadata;
    }

    private static ProductMetadata WithMinersSafetyRules(ProductMetadata metadata, string maxGroupSize)
    {
        metadata = WithRule(metadata, 1, "age", "minimum_age", "equals", "10");
        metadata = WithRule(metadata, 2, "supervision", "under_18", "requires", "adult_guardian");
        metadata = WithRule(metadata, 3, "guide", "guide_przodowy", "equals", "required");
        metadata = WithRule(metadata, 4, "capacity", "group_size", "max", maxGroupSize);
        metadata = WithRule(metadata, 5, "mobility", "mobility_impairment", "not_supported", "true");
        metadata = WithRule(metadata, 6, "physical", "uneven_ground_ladders_stairs", "requires", "good_mobility");
        metadata = WithRule(metadata, 7, "equipment", "full_shoes", "equals", "required");
        metadata = WithRule(metadata, 8, "safety", "training_and_signature", "equals", "required");
        metadata = WithRule(metadata, 9, "prohibited", "open_fire_flashlights_smoking_animals", "equals", "true");
        return metadata;
    }

    private static ProductMetadata WithRule(
        ProductMetadata metadata,
        int number,
        string kind,
        string parameter,
        string op,
        string value)
    {
        var prefix = $"rule.{number:000}";
        return metadata
            .With($"{prefix}.kind", kind)
            .With($"{prefix}.parameter", parameter)
            .With($"{prefix}.operator", op)
            .With($"{prefix}.value", value);
    }

    private static string[] CategoriesFor(ProductType product)
    {
        var metadata = product.Metadata();
        var categories = new List<string> { "wieliczka", "kopalnia-soli" };

        var route = metadata.GetOrDefault("route_code", "other");
        categories.Add(route);

        var variant = metadata.GetOrDefault("offer_variant", "variant");
        categories.Add(variant);

        var segment = product.FeatureTypes()
            .MandatoryFeatures()
            .FirstOrDefault(f => f.Name == "customer_segment");

        if (segment != null)
        {
            categories.Add("wariant");
        }

        if (variant.Contains("school", StringComparison.OrdinalIgnoreCase))
        {
            categories.Add("szkoły");
        }

        if (variant.Contains("group", StringComparison.OrdinalIgnoreCase))
        {
            categories.Add("grupy");
        }

        if (variant.Contains("family", StringComparison.OrdinalIgnoreCase))
        {
            categories.Add("rodziny");
        }

        return categories.Distinct().ToArray();
    }

    private static void Define(
        ProductRelationshipFactory factory,
        List<ProductRelationship> relationships,
        IProduct from,
        IProduct to,
        ProductRelationshipType type)
    {
        var result = factory.DefineFor(from.Id(), to.Id(), type);
        var relationship = result.GetSuccess();
        if (relationship != null)
        {
            relationships.Add(relationship);
        }
    }
}
