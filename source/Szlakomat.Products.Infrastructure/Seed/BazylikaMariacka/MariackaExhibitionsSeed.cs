using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Infrastructure.Seed.BazylikaMariacka;

/// <summary>
/// Seeds Bazylika Mariacka (St. Mary's Basilica) products as ProductType instances.
/// Two independent routes:
///   1. Zwiedzanie wnętrza Bazyliki — Mon–Sat 11:30–18:00, Sun/Hol 14:00–18:00
///   2. Hejnalica (North Tower) — Fri–Sun 10:10–17:30, entry every 10 min, seasonal from 10 April
/// These routes share the same location but are otherwise completely independent:
///   different hours, different prices, different restrictions, separate ticket sale.
/// Source: mariacki.com (stan na 2025).
/// </summary>
internal static class MariackaExhibitionsSeed
{
    // --- Shared feature types ---

    private static readonly ProductFeatureType TicketType =
        ProductFeatureType.WithAllowedValues("ticket_type", "standard", "reduced");

    private static readonly ProductFeatureType VisitDate =
        ProductFeatureType.WithDateRange("visit_date", "2025-04-10", "2026-12-31");

    // Visit slot: used by Hejnalica (entry every 10 minutes, timed entry required)
    private static readonly ProductFeatureType VisitTimeSlot =
        ProductFeatureType.WithAllowedValues(
            "visit_time_slot",
            "10:10", "10:20", "10:30", "10:40", "10:50",
            "11:00", "11:10", "11:20", "11:30", "11:40", "11:50",
            "12:00", "12:10", "12:20", "12:30", "12:40", "12:50",
            "13:00", "13:10", "13:20", "13:30", "13:40", "13:50",
            "14:00", "14:10", "14:20", "14:30", "14:40", "14:50",
            "15:00", "15:10", "15:20", "15:30", "15:40", "15:50",
            "16:00", "16:10", "16:20", "16:30", "16:40", "16:50",
            "17:00", "17:10", "17:20", "17:30");

    // -----------------------------------------------------------------------
    // Route 1: Zwiedzanie wnętrza Bazyliki
    // -----------------------------------------------------------------------
    // Open: Mon–Sat 11:30–18:00, Sun & public holidays 14:00–18:00
    // Ticket office closes 15 minutes before closing time.
    // The main altarpiece (ołtarz Wita Stwosza) opens daily at 11:50.
    // Standard: 20 PLN, Reduced: 10 PLN
    // Reduced eligibility: schoolchildren 7–18, students 19–26, seniors 65+, KDR card holders
    // -----------------------------------------------------------------------
    private static readonly IApplicabilityConstraint BasilikaInteriorAvailability =
        ApplicabilityConstraint.AlwaysTrue();
    // Note: day-of-week differentiation (weekday vs. Sunday/holiday hours) is encoded
    // in metadata, not as an applicability constraint, because the model lacks a
    // built-in DayOfWeek constraint type. A future constraint (e.g. DayOfWeekConstraint)
    // could enforce Sat vs. Sun opening separately.

    // -----------------------------------------------------------------------
    // Route 2: Hejnalica (North Tower)
    // -----------------------------------------------------------------------
    // Open: Fri–Sun 10:10–17:30 (entries every 10 minutes)
    // Season: from 10 April each year; closed in bad weather conditions
    // Max 15 persons per entry slot; children under 7: entry prohibited
    // Tickets: 20 PLN standard, 15 PLN reduced; on-site purchase only, no online booking
    // -----------------------------------------------------------------------
    // Applicability: visitor_age >= 7 (GreaterThan 6)
    private static readonly IApplicabilityConstraint HejnalicaAvailability =
        ApplicabilityConstraint.GreaterThan("visitor_age", 6);

    public static (ProductType BasilikaInterior, ProductType Hejnalica) Seed(
        IProductTypeRepository repo)
    {
        var basilikaInterior = BuildBasilikaInterior();
        var hejnalica = BuildHejnalica();

        repo.Save(basilikaInterior);
        repo.Save(hejnalica);

        return (basilikaInterior, hejnalica);
    }

    // -----------------------------------------------------------------------
    // Route 1 builder
    // -----------------------------------------------------------------------
    private static ProductType BuildBasilikaInterior()
    {
        var metadata = SeedHelpers.MariackaBase()
            .With("route_type", "interior")
            .With("avg_duration_min", "45")
            // Opening hours — weekdays & Saturday
            .With("open_mon_sat_from", "11:30")
            .With("open_mon_sat_to", "18:00")
            // Opening hours — Sunday & public holidays
            .With("open_sun_hol_from", "14:00")
            .With("open_sun_hol_to", "18:00")
            // Ticket office closes 15 min before the venue
            .With("ticket_office_closes_min_before", "15")
            // Main altarpiece schedule
            .With("altarpiece_opens_daily_at", "11:50")
            .With("altarpiece_note", "Ołtarz Wita Stwosza otwierany codziennie o 11:50")
            // Pricing
            .With("price_standard_pln", "20")
            .With("price_reduced_pln", "10")
            // Reduced eligibility
            .With("reduced_eligibility_schoolchildren", "7-18 lat")
            .With("reduced_eligibility_students", "19-26 lat")
            .With("reduced_eligibility_seniors", "65+")
            .With("reduced_eligibility_kdr", "Karta Dużej Rodziny")
            // Booking
            .With("online_booking_available", "true")
            .With("requires_timed_entry", "false")
            // Notes
            .With("note_dress_code", "Wymagany odpowiedni strój (zakryte ramiona i kolana)")
            .With("note_photography", "Zakaz fotografowania przy użyciu lampy błyskowej")
            .With("note_groups", "Grupy zorganizowane: wymagana rezerwacja z wyprzedzeniem");

        return new ProductBuilder(
                UuidProductIdentifier.Of("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"),
                ProductName.Of("Bazylika Mariacka — Zwiedzanie wnętrza"),
                ProductDescription.Of(
                    "Zwiedzanie wnętrza Bazyliki Mariackiej w Krakowie (kościół Wniebowzięcia NMP). " +
                    "Gotycka świątynia z XIV–XV w., znana z polichromii Jana Matejki i słynnego " +
                    "ołtarza Wita Stwosza otwieranego codziennie o 11:50. " +
                    "Wejście: Plac Mariacki 5. " +
                    "Godziny: Pon–Sob 11:30–18:00, Ndz i Święta 14:00–18:00 " +
                    "(kasa 15 min przed zamknięciem)."))
            .WithMetadata(metadata)
            .WithApplicabilityConstraint(BasilikaInteriorAvailability)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.IndividuallyTracked)
                .WithMandatoryFeature(TicketType)
                .WithMandatoryFeature(VisitDate)
                .Build();
    }

    // -----------------------------------------------------------------------
    // Route 2 builder
    // -----------------------------------------------------------------------
    private static ProductType BuildHejnalica()
    {
        var metadata = SeedHelpers.MariackaBase()
            .With("route_type", "tower")
            .With("tower_name", "Hejnalica (Wieża Północna)")
            .With("avg_duration_min", "30")
            // Opening hours — Fri, Sat, Sun only
            .With("open_days", "Pt, Sob, Ndz")
            .With("open_from", "10:10")
            .With("open_to", "17:30")
            .With("entry_interval_min", "10")
            .With("max_persons_per_entry", "15")
            // Season
            .With("season_start", "10 IV")
            .With("season_note", "Czynna od 10 kwietnia każdego roku")
            .With("weather_closure", "W razie złych warunków atmosferycznych Hejnalica jest nieczynna")
            // Pricing
            .With("price_standard_pln", "20")
            .With("price_reduced_pln", "15")
            // Booking & ticket purchase
            .With("online_booking_available", "false")
            .With("ticket_purchase_note", "Bilety wyłącznie w kasie w dniu wizyty — brak rezerwacji online")
            // Access restrictions
            .With("min_age", "7")
            .With("age_restriction_note", "Dzieci poniżej 7 lat: wstęp wzbroniony")
            .With("requires_timed_entry", "true")
            .With("stairs_note", "Dostęp wyłącznie po stromych schodach — brak windy")
            .With("accessibility_note", "Nieczynna dla osób z ograniczoną sprawnością ruchową")
            // Route independence note (for documentation purposes)
            .With("route_independent_from", "interior")
            .With("route_note",
                "Trasa niezależna od zwiedzania wnętrza — oddzielny bilet, inne godziny, " +
                "inne dni otwarcia. Wybór tej trasy nie wyklucza zwiedzania wnętrza i odwrotnie.");

        return new ProductBuilder(
                UuidProductIdentifier.Of("2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e"),
                ProductName.Of("Bazylika Mariacka — Hejnalica (Wieża Północna)"),
                ProductDescription.Of(
                    "Wejście na Hejnalicę — Wieżę Północną Bazyliki Mariackiej w Krakowie. " +
                    "Punkt widokowy z panoramą Rynku Głównego i Krakowa. " +
                    "Skąd co godzinę rozlega się słynny hejnał mariacki. " +
                    "Wejście: Plac Mariacki 5. " +
                    "Czynna: Pt–Ndz 10:10–17:30, wejścia co 10 min, max 15 osób. " +
                    "Sezon od 10 kwietnia. Bilety wyłącznie w kasie w dniu wizyty."))
            .WithMetadata(metadata)
            .WithApplicabilityConstraint(HejnalicaAvailability)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.BatchTracked)
                .WithMandatoryFeature(TicketType)
                .WithMandatoryFeature(VisitDate)
                .WithMandatoryFeature(VisitTimeSlot)
                .Build();
    }
}
