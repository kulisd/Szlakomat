using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Quantity;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Infrastructure.Seed.WislaCruise;

/// <summary>
/// Seeds Krakowska Żegluga Pasażerska offers:
/// sightseeing cruise (50 min), short cruise under Wawel (30 min), evening cruise (50 min),
/// evening cruise with drink package, cruise to Tyniec (~4.5 h), private catamaran (30 or 60 min),
/// school group cruise, and organised group cruise.
/// Departure point: Przystań Wawel, ul. Bulwar Czerwieński 3, Kraków.
/// </summary>
internal static class WislaRiversideCruiseSeed
{
    private static readonly DateOnly OfferStart = new(2026, 4, 1);
    private static readonly DateOnly OfferEnd   = new(2026, 9, 31);

    private static readonly ProductFeatureType CruiseDate =
        ProductFeatureType.WithDateRange("cruise_date", "2026-01-01", "2026-12-31");

    // ---------------------------------------------------------------------------
    // Departure-time arrays
    // ---------------------------------------------------------------------------

    private static readonly string[] SightseeingCruiseTimes =
    {
        "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00"
    };

    /// <summary>30-min under-Wawel cruise departures.</summary>
    private static readonly string[] ShortCruiseTimes =
    {
        "10:40", "11:20", "12:00", "12:40",
        "13:00", "13:40", "14:20", "15:00", "15:40",
        "16:00", "16:40", "17:20"
    };

    /// <summary>Evening cruise departures (after sunset).</summary>
    private static readonly string[] EveningCruiseTimes =
    {
        "18:00", "19:00", "20:00"
    };

    /// <summary>Tyniec cruise – one morning sailing per day.</summary>
    private static readonly string[] TyniecCruiseTimes =
    {
        "10:00"
    };

    // ---------------------------------------------------------------------------
    // Public entry point
    // ---------------------------------------------------------------------------

    public static void Seed(
        IProductTypeRepository productRepo,
        IPackageTypeRepository packageRepo,
        ICatalogEntryRepository catalogRepo,
        IProductRelationshipRepository relationshipRepo,
        ProductRelationshipFactory relationshipFactory)
    {
        var offers   = SeedOffers(productRepo);
        var packages = SeedPackages(packageRepo, offers);

        SeedCatalog(catalogRepo, offers, packages);
        SeedRelationships(relationshipRepo, relationshipFactory, offers, packages);
    }

    // ---------------------------------------------------------------------------
    // Offer builders
    // ---------------------------------------------------------------------------

    private static List<ProductType> SeedOffers(IProductTypeRepository repo)
    {
        var offers = new List<ProductType>
        {
            BuildSightseeingCruise50min(),
            BuildShortCruise30min(),
            BuildEveningCruise(),
            BuildEveningCruiseWithDrink(),
            BuildTyniecCruise(),
            BuildPrivateCatamaranCruise30min(),
            BuildPrivateCatamaranCruise60min(),
            BuildSchoolGroupCruise(),
            BuildOrganisedGroupCruise(),
        };

        foreach (var offer in offers)
        {
            repo.Save(offer);
        }

        return offers;
    }

    // ── 1. Sightseeing cruise – 50 minutes ──────────────────────────────────────

    private static ProductType BuildSightseeingCruise50min()
    {
        var metadata = CruiseBase()
            .With("offer_variant",        "individual_sightseeing")
            .With("duration_minutes",     "50")
            .With("route_code",           "sightseeing_krakow_loop")
            .With("route_waypoints",
                "Przystań Wawel > Zakole Wisły pod Wawelem > Klasztor Sióstr Norbertanek > " +
                "Salwator > Most Grunwaldzki > Kościół Na Skałce > Kazimierz > " +
                "Kładka Ojca Bernatka > Cricoteka > Przystań Wawel")
            .With("audio_guide",          "true")
            .With("bar_on_board",         "Nimfa only")
            .With("toilet_on_board",      "true")
            .With("vessels",              "Nimfa (max 200), Orka (max 40)")
            .With("price_normal_pln",     "69")
            .With("free_child_under_age", "3")
            .With("pets_allowed",         "true")

        metadata = WithCommonCruiseRules(metadata, maxPassengers: "200");

        return BuildOffer(
            UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222201"),
            "KZP Rejs po Wiśle – 50 minut",
            "Sightseeing cruise po Wiśle w Krakowie z audioprzewodnikiem. " +
            "Trasa mija Wawel, Kazimierz, bulwary i mosty. Odpływ z Przystani Wawel.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 1, 200),
            new[] { "PL", "EN" },
            new[] { "normal", "child_under_3_free_control" },
            SightseeingCruiseTimes,
            "individual",
            (1, 200),
            (0, 120));
    }

    // ── 2. Short cruise under Wawel – 30 minutes ────────────────────────────────

    private static ProductType BuildShortCruise30min()
    {
        var metadata = CruiseBase()
            .With("offer_variant",        "individual_short")
            .With("duration_minutes",     "30")
            .With("route_code",           "wawel_loop_30min")
            .With("route_waypoints",
                "Przystań Wawel > Most Dębnicki > Bulwary Wiślane > pod Wawelem > " +
                "Most Grunwaldzki > Most Kotlarski > Kładka Ojca Bernatka > Przystań Wawel")
            .With("vessels",              "Katamaran (max 12) or Orka (max 40)")
            .With("price_normal_pln",     "49")
            .With("free_child_under_age", "4")

        metadata = WithCommonCruiseRules(metadata, maxPassengers: "40");

        return BuildOffer(
            UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222202"),
            "KZP Rejs pod Wawelem – 30 minut",
            "Krótki 30-minutowy rejs statkiem pod Wawelem. " +
            "Zależnie od terminu i liczby pasażerów – Katamaran (do 12 osób) lub Orka (do 40 osób).",
            metadata,
            ApplicabilityConstraint.Between("group_size", 1, 40),
            new[] { "PL", "EN" },
            new[] { "normal", "child_under_4_free_control" },
            ShortCruiseTimes,
            "individual",
            (1, 40),
            (0, 120));
    }

    // ── 3. Evening cruise – 50 minutes ──────────────────────────────────────────

    private static ProductType BuildEveningCruise()
    {
        var metadata = CruiseBase()
            .With("offer_variant",              "evening_individual")
            .With("duration_minutes",           "50")
            .With("route_code",                 "sightseeing_krakow_loop")
            .With("time_of_day",                "evening")
            .With("vessels",                    "Katamaran (max 12) or Nimfa (max 200) or Orka (max 40)")
            .With("audio_guide",                "true")
            .With("toilet_on_board",            "true")
            .With("price_adult_12_99_pln",      "79")
            .With("price_child_4_12_pln",       "69")
            .With("free_child_under_age",        "4")

        metadata = WithCommonCruiseRules(metadata, maxPassengers: "200");

        return BuildOffer(
            UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222203"),
            "KZP Rejs Wieczorny po Wiśle – 50 minut",
            "Wieczorny 50-minutowy rejs statkiem po iluminowanym Krakowie. " +
            "Dzieci do 4 lat bezpłatnie. Dostęp do baru na Przystani Wawel.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 1, 200),
            new[] { "PL", "EN" },
            new[] { "adult_12_99", "child_4_12", "child_under_4_free_control" },
            EveningCruiseTimes,
            "individual",
            (1, 200),
            (0, 120));
    }

    // ── 4. Evening cruise WITH DRINK package ────────────────────────────────────

    private static ProductType BuildEveningCruiseWithDrink()
    {
        var metadata = CruiseBase()
            .With("offer_variant",              "evening_with_drink")
            .With("duration_minutes",           "50")
            .With("route_code",                 "sightseeing_krakow_loop")
            .With("time_of_day",                "evening")
            .With("vessels",                    "Nimfa (max 200) or Orka")
            .With("audio_guide",                "true")
            .With("toilet_on_board",            "true")
            .With("package_includes",           "50-min cruise + 1 drink from bar menu")
            .With("price_adult_only_pln",       "119")
            .With("free_child_under_age",        "4")

        metadata = WithCommonCruiseRules(metadata, maxPassengers: "200");
        metadata = WithRule(metadata, 10, "alcohol", "minimum_age",  "equals",      "18");

        return BuildOffer(
            UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222204"),
            "KZP Pakiet: Rejs Wieczorny z Drinkiem",
            "50-minutowy wieczorny rejs po Wiśle + 1 drink z karty napojów baru Przystań Wawel. " +
            "Wyłącznie dla dorosłych (18+). Serwowanie alkoholu wymaga okazania dokumentu tożsamości.",
            metadata,
            ApplicabilityConstraint.And(
                ApplicabilityConstraint.Between("group_size",   1,   200),
                ApplicabilityConstraint.Between("visitor_age", 18,   120)),
            new[] { "PL", "EN" },
            new[] { "adult_18_plus_with_drink" },
            EveningCruiseTimes,
            "individual",
            (1, 200),
            (18, 120));
    }

    // ── 5. Cruise to Tyniec – ~4.5 hours ────────────────────────────────────────

    private static ProductType BuildTyniecCruise()
    {
        var metadata = CruiseBase()
            .With("offer_variant",       "day_trip_tyniec")
            .With("duration_hours",      "4.5")
            .With("duration_note",
                "May extend 30–60 min depending on lock-gate (śluzowanie) conditions; " +
                "exact stopover time announced by captain")
            .With("route_code",          "krakow_tyniec_return")
            .With("route_waypoints",
                "Przystań Wawel > Klasztor Sióstr Norbertanek > Kopiec Kościuszki > " +
                "Kościół Kamedułów na Bielanach > Stopień Wodny Kościuszko > " +
                "Opactwo Benedyktynów w Tyńcu (ok. 45 min wolnego czasu) > " +
                "powrót Przystań Wawel")
            .With("audio_guide",         "true")
            .With("free_water_bottle",   "true")
            .With("minimum_passengers",  "4")
            .With("price_adult_pln",     "160")
            .With("price_child_pln",     "150")

        metadata = WithCommonCruiseRules(metadata, maxPassengers: "200");
        metadata = WithRule(metadata, 10, "minimum_group", "passengers",       "min",    "4");

        return BuildOffer(
            UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222205"),
            "KZP Rejs do Tyńca – Kraków–Tyniec–Kraków",
            "Całodniowy rejs po Wiśle do Opactwa Benedyktynów w Tyńcu (~4,5 h, ok. 45 min postoju). " +
            "Butelka wody gratis. Minimalnie 4 pasażerów – operator może odwołać przy zbyt małej grupie.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 1, 200),
            new[] { "PL", "EN" },
            new[] { "adult", "child_reduced" },
            TyniecCruiseTimes,
            "individual",
            (1, 200),
            (0, 120));
    }

    // ── 6. Private catamaran – 30 minutes ────────────────────────────────────────

    private static ProductType BuildPrivateCatamaranCruise30min()
    {
        var metadata = PrivateCatamaranBase()
            .With("offer_variant",   "private_exclusive_30min")
            .With("duration_minutes","30")
            .With("price_total_pln", "450")
            .With("pricing_note",    "Flat rate for the whole vessel; not per person");

        metadata = WithPrivateCruiseRules(metadata);

        return BuildOffer(
            UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222206"),
            "KZP Prywatny Rejs Katamaranem – 30 minut",
            "Prywatny 30-minutowy rejs katamaranem na wyłączność (do 11 osób) z marynarzem. " +
            "Możliwość ustalenia własnej trasy. Cena: 450 PLN za cały rejs.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 1, 11),
            new[] { "PL", "EN" },
            new[] { "flat_rate_vessel" },
            ShortCruiseTimes,
            "private_exclusive",
            (1, 11),
            (0, 120));
    }

    // ── 7. Private catamaran – 60 minutes ────────────────────────────────────────

    private static ProductType BuildPrivateCatamaranCruise60min()
    {
        var metadata = PrivateCatamaranBase()
            .With("offer_variant",   "private_exclusive_60min")
            .With("duration_minutes","60")
            .With("price_total_pln", "850")
            .With("pricing_note",    "Flat rate for the whole vessel; not per person");

        metadata = WithPrivateCruiseRules(metadata);

        return BuildOffer(
            UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222207"),
            "KZP Prywatny Rejs Katamaranem – 60 minut",
            "Prywatny 60-minutowy rejs katamaranem na wyłączność (do 11 osób) z marynarzem. " +
            "Możliwość ustalenia własnej trasy. Cena: 850 PLN za cały rejs.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 1, 11),
            new[] { "PL", "EN" },
            new[] { "flat_rate_vessel" },
            SightseeingCruiseTimes,
            "private_exclusive",
            (1, 11),
            (0, 120));
    }

    // ── 8. School group cruise ────────────────────────────────────────────────────

    private static ProductType BuildSchoolGroupCruise()
    {
        var metadata = CruiseBase()
            .With("offer_variant",         "school_educational")
            .With("duration_minutes",      "50")
            .With("route_code",            "sightseeing_krakow_loop")
            .With("audio_guide",           "true")
            .With("educational_commentary","true")
            .With("booking_channel",       "email_or_phone")
            .With("price_note",            "Group pricing on request – contact operator");

        metadata = WithCommonCruiseRules(metadata, maxPassengers: "200");
        metadata = WithRule(metadata, 10, "school", "group_booking",  "requires", "prior_contact_with_operator");

        return BuildOffer(
            UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222208"),
            "KZP Rejs Szkolny po Wiśle",
            "Edukacyjny rejs po Wiśle dla szkół i grup zorganizowanych z komentarzem historycznym. " +
            "Cennik grupowy – prosimy o kontakt z operatorem.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 10, 200),
            new[] { "PL" },
            new[] { "school_group_rate", "teacher_supervisor_control" },
            SightseeingCruiseTimes,
            "school",
            (10, 200),
            (0, 120));
    }

    // ── 9. Organised group cruise ─────────────────────────────────────────────────

    private static ProductType BuildOrganisedGroupCruise()
    {
        var metadata = CruiseBase()
            .With("offer_variant",    "organised_group")
            .With("duration_minutes", "50")
            .With("route_code",       "sightseeing_krakow_loop")
            .With("audio_guide",      "true")
            .With("guide_on_request", "true")
            .With("booking_channel",  "email_or_phone")

        metadata = WithCommonCruiseRules(metadata, maxPassengers: "200");
        metadata = WithRule(metadata, 10, "group", "booking", "requires", "prior_contact_with_operator");

        return BuildOffer(
            UuidProductIdentifier.Of("22222222-2222-4222-8222-222222222209"),
            "KZP Rejs Grupowy po Wiśle",
            "Rejs wycieczkowy dla zorganizowanych grup (firmowe, turystyczne, okolicznościowe). " +
            "Możliwość wynajmu całego statku. Cennik grupowy – prosimy o kontakt z operatorem.",
            metadata,
            ApplicabilityConstraint.Between("group_size", 10, 200),
            new[] { "PL", "EN" },
            new[] { "group_rate", "guide_control" },
            SightseeingCruiseTimes,
            "organised_group",
            (10, 200),
            (0, 120));
    }

    // ---------------------------------------------------------------------------
    // Package builders
    // ---------------------------------------------------------------------------

    private static List<PackageType> SeedPackages(IPackageTypeRepository repo, List<ProductType> offers)
    {
        var byName = offers.ToDictionary(o => o.Name().Value);

        var shortCruise   = byName["KZP Rejs pod Wawelem – 30 minut"];
        var eveningCruise = byName["KZP Rejs Wieczorny po Wiśle – 50 minut"];
        var eveningDrink  = byName["KZP Pakiet: Rejs Wieczorny z Drinkiem"];

        var packages = new List<PackageType>
        {
            BuildPackage(
                UuidProductIdentifier.Of("22222222-2222-4222-8333-222222222301"),
                "KZP Pakiet: Rejs Wieczorny z Drinkiem – kompozyt",
                "Kompozyt wieczornego rejsu po Wiśle z drinkiem z baru Przystań Wawel (18+).",
                CruiseBase()
                    .With("package_type",    "evening_cruise_plus_drink")
                    .With("price_total_pln", "119")
                    .With("adult_only",      "true"),
                ProductTrackingStrategy.IndividuallyTracked,
                b => b.WithSingleChoice("evening_cruise_with_drink", eveningDrink.Id())),

            BuildPackage(
                UuidProductIdentifier.Of("22222222-2222-4222-8333-222222222302"),
                "KZP Pakiet: Regionalna Kolacja z Rejsem 30 min",
                "Polska kolacja regionalna w restauracji Aquarius pod Wawelem + 30-minutowy rejs po Wiśle.",
                CruiseBase()
                    .With("package_type",         "dinner_plus_cruise")
                    .With("restaurant",           "Restauracja Aquarius, ul. Bulwar Czerwieński 3, Kraków")
                ProductTrackingStrategy.IndividuallyTracked,
                b => b.WithSingleChoice("cruise", shortCruise.Id())),

            BuildPackage(
                UuidProductIdentifier.Of("22222222-2222-4222-8333-222222222303"),
                "KZP Pakiet: Romantyczna Degustacyjna Kolacja z Rejsem",
                "Romantyczna kolacja degustacyjna w restauracji Aquarius pod Wawelem + 30-minutowy rejs po Wiśle.",
                CruiseBase()
                    .With("package_type",         "romantic_dinner_plus_cruise")
                    .With("restaurant",           "Restauracja Aquarius, ul. Bulwar Czerwieński 3, Kraków")
                ProductTrackingStrategy.IndividuallyTracked,
                b => b.WithSingleChoice("cruise", shortCruise.Id())),

            BuildPackage(
                UuidProductIdentifier.Of("22222222-2222-4222-8333-222222222304"),
                "KZP Pakiet: Wieczór na Wiśle – rejs + opcjonalny drink",
                "Wieczorny rejs po Wiśle z opcją dokupienia drinka z baru Przystań Wawel.",
                CruiseBase()
                    .With("package_type", "evening_cruise_optional_drink")
                    .With("drink_note",   "Optional upgrade to evening_with_drink variant available at bar"),
                ProductTrackingStrategy.IndividuallyTracked,
                b => b
                    .WithSingleChoice("evening_cruise",   eveningCruise.Id())
                    .WithOptionalChoice("drink_upgrade",  eveningDrink.Id())),
        };

        foreach (var package in packages)
        {
            repo.Save(package);
        }

        return packages;
    }

    // ---------------------------------------------------------------------------
    // Catalog seeding
    // ---------------------------------------------------------------------------

    private static void SeedCatalog(
        ICatalogEntryRepository repo,
        List<ProductType> offers,
        List<PackageType> packages)
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
                ("source", "e-statek.pl")));
        }

        foreach (var package in packages)
        {
            entries.Add(BuildEntry(
                package,
                package.Name().Value,
                package.Description().Value,
                new[] { "wisla", "krakow", "rejs-wislany", "pakiet" },
                Validity.Between(OfferStart, OfferEnd),
                ("source",      "e-statek.pl"),
                ("composition", "package_type")));
        }

        foreach (var entry in entries)
        {
            repo.Save(entry);
        }
    }

    // ---------------------------------------------------------------------------
    // Relationship seeding
    // ---------------------------------------------------------------------------

    private static void SeedRelationships(
        IProductRelationshipRepository repo,
        ProductRelationshipFactory factory,
        List<ProductType> offers,
        List<PackageType> packages)
    {
        var offer   = offers.ToDictionary(o => o.Name().Value);
        var package = packages.ToDictionary(p => p.Name().Value);
        var relationships = new List<ProductRelationship>();

        // Short cruise is the entry point for both dinner packages
        Define(factory, relationships,
            offer["KZP Rejs pod Wawelem – 30 minut"],
            package["KZP Pakiet: Regionalna Kolacja z Rejsem 30 min"],
            ProductRelationshipType.UpgradableTo);

        Define(factory, relationships,
            offer["KZP Rejs pod Wawelem – 30 minut"],
            package["KZP Pakiet: Romantyczna Degustacyjna Kolacja z Rejsem"],
            ProductRelationshipType.UpgradableTo);

        // Evening cruise can be upgraded to include a drink
        Define(factory, relationships,
            offer["KZP Rejs Wieczorny po Wiśle – 50 minut"],
            offer["KZP Pakiet: Rejs Wieczorny z Drinkiem"],
            ProductRelationshipType.UpgradableTo);

        Define(factory, relationships,
            offer["KZP Rejs Wieczorny po Wiśle – 50 minut"],
            package["KZP Pakiet: Wieczór na Wiśle – rejs + opcjonalny drink"],
            ProductRelationshipType.UpgradableTo);

        // Private catamaran 30 min is upgradable to 60 min
        Define(factory, relationships,
            offer["KZP Prywatny Rejs Katamaranem – 30 minut"],
            offer["KZP Prywatny Rejs Katamaranem – 60 minut"],
            ProductRelationshipType.UpgradableTo);

        // Sightseeing cruise and Tyniec cruise complement each other
        Define(factory, relationships,
            offer["KZP Rejs po Wiśle – 50 minut"],
            offer["KZP Rejs do Tyńca – Kraków–Tyniec–Kraków"],
            ProductRelationshipType.ComplementedBy);

        // Group and school variants substitute the standard individual cruise
        Define(factory, relationships,
            offer["KZP Rejs po Wiśle – 50 minut"],
            offer["KZP Rejs Grupowy po Wiśle"],
            ProductRelationshipType.SubstitutedBy);

        Define(factory, relationships,
            offer["KZP Rejs po Wiśle – 50 minut"],
            offer["KZP Rejs Szkolny po Wiśle"],
            ProductRelationshipType.SubstitutedBy);

        // The two dinner packages are compatible alternatives to each other
        Define(factory, relationships,
            package["KZP Pakiet: Regionalna Kolacja z Rejsem 30 min"],
            package["KZP Pakiet: Romantyczna Degustacyjna Kolacja z Rejsem"],
            ProductRelationshipType.CompatibleWith);

        foreach (var relationship in relationships)
        {
            repo.Save(relationship);
        }
    }

    // ---------------------------------------------------------------------------
    // Low-level builders – identical signatures to WieliczkaSaltMineSeed
    // ---------------------------------------------------------------------------

    private static ProductType BuildOffer(
        IProductIdentifier id,
        string name,
        string description,
        ProductMetadata metadata,
        IApplicabilityConstraint applicability,
        string[] languages,
        string[] ticketTypes,
        string[] departureTimes,
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
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("ticket_type",      ticketTypes))
                .WithMandatoryFeature(CruiseDate)
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("departure_time",   departureTimes))
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("language",         languages))
                .WithMandatoryFeature(ProductFeatureType.WithAllowedValues("customer_segment", customerSegment))
                .WithMandatoryFeature(ProductFeatureType.WithNumericRange("group_size",  groupSizeRange.Min,  groupSizeRange.Max))
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

    // ---------------------------------------------------------------------------
    // Metadata base builders
    // ---------------------------------------------------------------------------

    private static ProductMetadata CruiseBase() =>
        ProductMetadata.Empty()
            .With("city",             "Kraków")
            .With("region",           "Małopolska")
            .With("operator",         "Krakowska Żegluga Pasażerska")
            .With("category",         "rejs-wislany")
            .With("departure_point",  "Przystań Wawel")
            .With("departure_address","ul. Bulwar Czerwieński 3, 30-001 Kraków")
            .With("departure_lat",    "50.054500")
            .With("departure_lon",    "19.935600")

    private static ProductMetadata PrivateCatamaranBase() =>
        CruiseBase()
            .With("vessel",             "Katamaran")
            .With("max_passengers",     "11")
            .With("exclusive_hire",     "true")
            .With("custom_route",       "negotiable_with_captain")

    // ---------------------------------------------------------------------------
    // Rule helpers – identical pattern to WieliczkaSaltMineSeed
    // ---------------------------------------------------------------------------

    private static ProductMetadata WithCommonCruiseRules(
        ProductMetadata metadata,
        string maxPassengers)
    {
        metadata = WithRule(metadata, 1, "capacity",   "max_passengers",      "max",    maxPassengers);
        metadata = WithRule(metadata, 2, "safety",     "life_jackets",        "equals", "provided");
        metadata = WithRule(metadata, 3, "safety",     "professional_crew",   "equals", "required");
        metadata = WithRule(metadata, 4, "prohibited", "disruptive_behaviour","equals", "refused_boarding");
        metadata = WithRule(metadata, 5, "cancellation","refund_window_hours", "equals", "24");
        return metadata;
    }

    private static ProductMetadata WithPrivateCruiseRules(ProductMetadata metadata)
    {
        metadata = WithCommonCruiseRules(metadata, maxPassengers: "11");
        metadata = WithRule(metadata, 10, "exclusive", "vessel_hire",    "equals",      "full_vessel_exclusive");
        metadata = WithRule(metadata, 11, "booking",   "advance_booking","recommended", "true");
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
            .With($"{prefix}.kind",      kind)
            .With($"{prefix}.parameter", parameter)
            .With($"{prefix}.operator",  op)
            .With($"{prefix}.value",     value);
    }

    // ---------------------------------------------------------------------------
    // Category helper
    // ---------------------------------------------------------------------------

    private static string[] CategoriesFor(ProductType product)
    {
        var metadata   = product.Metadata();
        var categories = new List<string> { "wisla", "krakow", "rejs-wislany" };

        var variant = metadata.GetOrDefault("offer_variant", "variant");
        categories.Add(variant);

        if (variant.Contains("evening", StringComparison.OrdinalIgnoreCase))
            categories.Add("wieczorny");

        if (variant.Contains("school", StringComparison.OrdinalIgnoreCase))
            categories.Add("szkoły");

        if (variant.Contains("group", StringComparison.OrdinalIgnoreCase))
            categories.Add("grupy");

        if (variant.Contains("private", StringComparison.OrdinalIgnoreCase))
            categories.Add("prywatny");

        var routeCode = metadata.GetOrDefault("route_code", "");
        if (routeCode.Contains("tyniec", StringComparison.OrdinalIgnoreCase))
            categories.Add("tyniec");

        return categories.Distinct().ToArray();
    }

    // ---------------------------------------------------------------------------
    // Relationship helper – identical signature to WieliczkaSaltMineSeed
    // ---------------------------------------------------------------------------

    private static void Define(
        ProductRelationshipFactory factory,
        List<ProductRelationship> relationships,
        IProduct from,
        IProduct to,
        ProductRelationshipType type)
    {
        var result       = factory.DefineFor(from.Id(), to.Id(), type);
        var relationship = result.GetSuccess();
        if (relationship != null)
        {
            relationships.Add(relationship);
        }
    }
}
