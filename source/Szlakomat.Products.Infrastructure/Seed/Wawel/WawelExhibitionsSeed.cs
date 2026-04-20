using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Infrastructure.Seed.Wawel;

/// <summary>
/// Seeds Wawel Royal Castle exhibitions as ProductType instances.
/// Exhibitions and names aligned with wawel.krakow.pl (stan na 2025).
/// </summary>
internal static class WawelExhibitionsSeed
{
    // Shared feature definitions for all exhibitions
    private static readonly ProductFeatureType TicketType =
        ProductFeatureType.WithAllowedValues("ticket_type", "standard", "reduced", "group", "free");

    private static readonly ProductFeatureType VisitDate =
        ProductFeatureType.WithDateRange("visit_date", "2025-01-01", "2026-12-31");

    private static readonly ProductFeatureType Language =
        ProductFeatureType.WithAllowedValues("language", "PL", "EN", "UA", "DE", "FR", "IT", "ES");

    private static readonly ProductFeatureType AudioGuide =
        ProductFeatureType.Unconstrained("audio_guide", FeatureValueType.Boolean);

    // Monday free access: selected exhibitions rotate seasonally (10:00-16:00)
    // Seasons:
    //   2 I  – 23 III: Podziemia Zamku + Zbrojownia
    //   30 III – 30 VI: Zamek piętro 1 + Zbrojownia + Ogrody Królewskie
    //   1 VII – 30 IX: Podziemia Zamku + Międzymurze + Ogrody Królewskie
    //   1 X  – 31 XII: Zamek piętro 2 + Skarbiec
    // Modeled as: no blanket ClosedMondays; individual exhibitions use MondayFreeAccess
    // or are simply unavailable on Mondays depending on the season.
    //
    // For seed purposes we use AlwaysTrue (no day-of-week block) since Monday
    // access depends on season rotation and ticket_type=free, not a blanket closure.
    private static readonly IApplicabilityConstraint StandardAvailability =
        ApplicabilityConstraint.AlwaysTrue();

    public static List<ProductType> Seed(IProductTypeRepository repo)
    {
        var exhibitions = new List<ProductType>
        {
            // --- Zamek piętro 1 (Prywatne Apartamenty Królewskie) ---
            BuildExhibition(
                InspireProductIdentifier.Of("PL.1.9.ZIPOZ.NID_N_12_BK.217616"),
                "Prywatne Apartamenty Królewskie",
                "Prywatne apartamenty królewskie na I piętrze Zamku Królewskiego na Wawelu, w tym Gabinet Porcelanowy.",
                floor: "1", avgDuration: "60",
                extraMetadata: m => m
                    .With("price_standard_pln", "57")
                    .With("price_reduced_pln", "43")
                    .With("last_entry_min_before_close", "60")
                    .With("includes", "Gabinet Porcelanowy")
                    .With("closure_2025", "2-13 III, 4-6 IV")
                    .With("monday_free_season", "30 III - 30 VI")
                    .With("wawel_odzyskany_access", "true")),

            // --- Zamek piętro 2 (Reprezentacyjne Komnaty Królewskie) ---
            BuildExhibition(
                InspireProductIdentifier.Of("PL.1.9.ZIPOZ.NID_N_12_BK.217617"),
                "Reprezentacyjne Komnaty Królewskie",
                "Reprezentacyjne sale królewskie na II piętrze z arrasami, malowidłami i Namiotami Tureckimi.",
                floor: "2", avgDuration: "60",
                extraMetadata: m => m
                    .With("price_standard_pln", "57")
                    .With("price_reduced_pln", "43")
                    .With("last_entry_min_before_close", "50")
                    .With("includes", "Namioty Tureckie")
                    .With("closure_2025", "2-13 III, 4-6 IV")
                    .With("namioty_tureckie_closure", "8-26 VI")
                    .With("monday_free_season", "1 X - 31 XII")
                    .With("wawel_odzyskany_access", "true")),

            // --- Skarbiec Koronny ---
            BuildExhibition(
                InspireProductIdentifier.Of("PL.1.9.ZIPOZ.NID_N_12_BK.217618"),
                "Skarbiec Koronny",
                "Insygnia koronacyjne, regalia i precjoza królewskie.",
                floor: "0", avgDuration: "40",
                extraMetadata: m => m
                    .With("price_standard_pln", "47")
                    .With("price_reduced_pln", "35")
                    .With("last_entry_min_before_close", "40")
                    .With("closure_2025", "16-27 III, 4-6 IV")
                    .With("monday_free_season", "1 X - 31 XII")
                    .With("pjm_available", "true")),

            // --- Zbrojownia ---
            BuildExhibition(
                InspireProductIdentifier.Of("PL.1.9.ZIPOZ.NID_N_12_BK.217619"),
                "Zbrojownia",
                "Kolekcja uzbrojenia i zbroi z XV–XVIII w.",
                floor: "0", avgDuration: "40",
                extraMetadata: m => m
                    .With("price_standard_pln", "47")
                    .With("price_reduced_pln", "35")
                    .With("last_entry_min_before_close", "30")
                    .With("closure_2025", "4-5 IV")
                    .With("monday_free_season", "2 I - 23 III, 30 III - 30 VI")
                    .With("easter_2025_free", "6 IV 09:00-17:00")),

            // --- Podziemia Zamku (Wawel Zaginiony + Lapidarium + Kościół św. Gereona) ---
            BuildExhibition(
                InspireProductIdentifier.Of("PL.1.9.ZIPOZ.NID_N_12_BK.217620"),
                "Podziemia Zamku",
                "Podziemna trasa archeologiczna: Wawel Zaginiony, Lapidarium, relikty najstarszych budowli wzgórza wawelskiego. Latem dodatkowo Kościół św. Gereona.",
                floor: "-1", avgDuration: "60",
                extraMetadata: m => m
                    .With("audio_in_price", "true")
                    .With("price_standard_pln", "47")
                    .With("price_reduced_pln", "35")
                    .With("last_entry_min_before_close", "60")
                    .With("includes", "Wawel Zaginiony, Lapidarium")
                    .With("summer_includes", "Kościół św. Gereona")
                    .With("closure_2025", "24 III - 6 IV")
                    .With("monday_free_season", "2 I - 23 III, 1 VII - 30 IX")
                    .With("pjm_available", "true")
                    .With("audioguide_pickup", "wejście na Wawel Zaginiony")),

            // --- Międzymurze. Podziemia Wawelu ---
            BuildExhibition(
                InspireProductIdentifier.Of("PL.1.9.ZIPOZ.NID_N_12_BK.217621"),
                "Międzymurze. Podziemia Wawelu",
                "Trasa podziemna i spacerowa między murami obronnymi Wawelu.",
                floor: "0", avgDuration: "30",
                extraMetadata: m => m
                    .With("audio_in_price", "true")
                    .With("indoor_outdoor", "mixed")
                    .With("price_standard_pln", "29")
                    .With("price_reduced_pln", "22")
                    .With("last_entry_min_before_close", "30")
                    .With("closure_2025", "4-6 IV")
                    .With("monday_free_season", "1 VII - 30 IX")
                    .With("audioguide_pickup", "przy wejściu")),

            // --- Ogrody Królewskie (sezonowe, 24 IV - 4 X) ---
            // Sezonowe ogrody nie są samodzielnym zabytkiem NID — UUID zamiast INSPIRE ID.
            BuildExhibition(
                UuidProductIdentifier.Of("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                "Ogrody Królewskie",
                "Ogrody królewskie na Wawelu — dostępne sezonowo od 24 kwietnia do 4 października.",
                floor: "0", avgDuration: "30",
                extraMetadata: m => m
                    .With("indoor_outdoor", "outdoor")
                    .With("seasonal", "true")
                    .With("season_start", "24 IV")
                    .With("season_end", "4 X")
                    .With("price_standard_pln", "0")
                    .With("price_reduced_pln", "0")
                    .With("monday_free_season", "30 III - 30 VI, 1 VII - 30 IX")),

            // --- Wystawa czasowa aktualna: Skarby z Łowicza ---
            // Wystawy czasowe nie mają numeru NID — UUID zamiast INSPIRE ID.
            BuildExhibition(
                UuidProductIdentifier.Of("7f3b2a1e-9c4d-4e8f-b5a6-1d2c3e4f5a6b"),
                "Skarby z Łowicza",
                "Wystawa czasowa prezentująca zbiory pochodzące z Łowicza.",
                floor: "0", avgDuration: "30",
                extraMetadata: m => m
                    .With("temporary_exhibition", "true")
                    .With("status", "current")
                    .With("price_standard_pln", "5")
                    .With("price_reduced_pln", "3")),

            // --- Wawel Odzyskany (budynek nr 7) ---
            // Ekspozycja w budynku zamku, nie samodzielny zabytek NID — UUID zamiast INSPIRE ID.
            BuildExhibition(
                UuidProductIdentifier.Of("2e1d4c3b-8f7a-4d6e-a5b4-9c8d7e6f5a4b"),
                "Wawel Odzyskany",
                "Ekspozycja w budynku nr 7 — dostępna z biletem na Zamek (piętro 1 lub 2).",
                floor: "0", avgDuration: "20",
                extraMetadata: m => m
                    .With("requires_castle_ticket", "true")
                    .With("price_standard_pln", "0")
                    .With("price_reduced_pln", "0")
                    .With("building", "7")),

            // --- Smocza Jama (sezonowo 24 IV - 31 X) ---
            // Naturalna jaskinia — atrakcja turystyczna, nie zabytek w rejestrze NID — UUID zamiast INSPIRE ID.
            BuildExhibition(
                UuidProductIdentifier.Of("5c4b3a2d-1e0f-4a9b-8c7d-6e5f4a3b2c1d"),
                "Smocza Jama",
                "Legendarna jaskinia smoka wawelskiego u stóp wzgórza, dostępna sezonowo.",
                floor: "-1", avgDuration: "15",
                extraMetadata: m => m
                    .With("indoor_outdoor", "outdoor")
                    .With("seasonal", "true")
                    .With("season_start", "24 IV")
                    .With("season_end", "31 X")
                    .With("price_standard_pln", "15")
                    .With("price_reduced_pln", "10")),

            // --- Baszta Widokowa (sezonowo 24 IV - 31 X) ---
            // Punkt widokowy bez odrębnego numeru NID — UUID zamiast INSPIRE ID.
            BuildExhibition(
                UuidProductIdentifier.Of("8a7b6c5d-4e3f-4b2a-1c0d-9e8f7a6b5c4d"),
                "Baszta Widokowa",
                "Punkt widokowy na wzgórzu wawelskim z panoramą Krakowa, dostępny sezonowo.",
                floor: "0", avgDuration: "20",
                extraMetadata: m => m
                    .With("indoor_outdoor", "outdoor")
                    .With("seasonal", "true")
                    .With("season_start", "24 IV")
                    .With("season_end", "31 X")
                    .With("price_standard_pln", "19")
                    .With("price_reduced_pln", "14")),

            // --- Baszta Sandomierska (zamknięta na remont konserwatorski) ---
            BuildExhibition(
                InspireProductIdentifier.Of("PL.1.9.ZIPOZ.NID_N_12_BK.217627"),
                "Baszta Sandomierska",
                "Zabytkowa baszta obronna Wawelu — obecnie zamknięta z powodu remontu konserwatorskiego.",
                floor: "0", avgDuration: "20",
                extraMetadata: m => m
                    .With("indoor_outdoor", "outdoor")
                    .With("status", "closed_for_renovation")
                    .With("closure_reason", "remont konserwatorski")
                    .With("closure_from", "25 III 2025")),

            // --- Wystawa czasowa planowana: Przepraszam za bałagan. Michalina Bigaj ---
            // Wystawy czasowe nie mają numeru NID — UUID zamiast INSPIRE ID.
            BuildExhibition(
                UuidProductIdentifier.Of("9b8a7c6d-5f4e-4c3b-2d1e-0f9a8b7c6d5e"),
                "Przepraszam za bałagan. Michalina Bigaj",
                "Planowana wystawa czasowa artystki Michaliny Bigaj.",
                floor: "0", avgDuration: "30",
                extraMetadata: m => m
                    .With("temporary_exhibition", "true")
                    .With("status", "planned")
                    .With("planned_year", "2026")),
        };

        foreach (var exhibition in exhibitions)
        {
            repo.Save(exhibition);
        }

        return exhibitions;
    }

    private static ProductType BuildExhibition(
        IProductIdentifier id,
        string name,
        string description,
        string floor,
        string avgDuration,
        Func<ProductMetadata, ProductMetadata>? extraMetadata = null)
    {
        var metadata = SeedHelpers.WawelExhibitionBase()
            .With("exhibition_floor", floor)
            .With("avg_duration_min", avgDuration);

        if (extraMetadata != null)
        {
            metadata = extraMetadata(metadata);
        }

        return new ProductBuilder(
                id,
                ProductName.Of(name),
                ProductDescription.Of(description))
            .WithMetadata(metadata)
            .WithApplicabilityConstraint(StandardAvailability)
            .AsProductType(Unit.Pieces(), ProductTrackingStrategy.IndividuallyTracked)
                .WithMandatoryFeature(TicketType)
                .WithMandatoryFeature(VisitDate)
                .WithMandatoryFeature(Language)
                .WithOptionalFeature(AudioGuide)
                .Build();
    }
}
