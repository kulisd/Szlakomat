using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Infrastructure.Seed.KopiecKosciuszki;

/// <summary>
/// Seeds Kopiec Kościuszki attractions as ProductType instances.
/// Data aligned with fundacja-kopiec.pl (stan na 2025).
/// Kopiec zarządzany przez Społeczny Komitet Odnowy Kopca Kościuszki / Fundację Kopiec Kościuszki.
/// </summary>
internal static class KopiecKosciuszkiExhibitionsSeed
{
    // Shared feature definitions
    private static readonly ProductFeatureType TicketType =
        ProductFeatureType.WithAllowedValues("ticket_type", "normalny", "ulgowy", "rodzinny", "bezpłatny");

    private static readonly ProductFeatureType VisitDate =
        ProductFeatureType.WithDateRange("visit_date", "2025-01-01", "2026-12-31");

    private static readonly ProductFeatureType Language =
        ProductFeatureType.WithAllowedValues("language", "PL", "EN", "DE", "UA", "FR");

    private static readonly ProductFeatureType AudioGuide =
        ProductFeatureType.Unconstrained("audio_guide", FeatureValueType.Boolean);

    // Kopiec is open year-round; fort/museum has seasonal hours
    private static readonly IApplicabilityConstraint StandardAvailability =
        ApplicabilityConstraint.AlwaysTrue();

    public static List<ProductType> Seed(IProductTypeRepository repo)
    {
        var attractions = new List<ProductType>
        {
            // --- Wejście na Kopiec Kościuszki (wejście główne + trasa spiralna) ---
            // Kopiec wpisany do rejestru zabytków: NID A-980 (obiekt zabytkowy Krakowa)
            BuildAttraction(
                InspireProductIdentifier.Of("PL.1.9.ZIPOZ.NID_A_12_ZR.980"),
                "Wejście na Kopiec Kościuszki",
                "Wejście na Kopiec Kościuszki spiralną ścieżką (150 schodów, ~34 m). " +
                "Na szczycie platforma widokowa z panoramą 360° Krakowa i Tatr.",
                zone: "kopiec", avgDuration: "30",
                extraMetadata: m => m
                    .With("price_normalny_pln", "16")
                    .With("price_ulgowy_pln", "10")
                    .With("indoor_outdoor", "outdoor")
                    .With("height_m", "34")
                    .With("steps_count", "150")
                    .With("panorama_360", "true")
                    .With("accessibility", "none")
                    .With("last_entry_min_before_close", "30")
                    .With("open_mon_fri", "09:00-18:00")
                    .With("open_sat_sun", "09:00-20:00")),

            // --- Muzeum im. Tadeusza Kościuszki (w forcie, budynek Rotundy) ---
            // Muzeum w budynku Rotundy fortu — własna ekspozycja stała
            BuildAttraction(
                UuidProductIdentifier.Of("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                "Muzeum im. Tadeusza Kościuszki",
                "Ekspozycja stała poświęcona życiu i działalności Tadeusza Kościuszki: " +
                "pamiątki, dokumenty, militaria, mundury z epoki insurekcji kościuszkowskiej 1794 r.",
                zone: "fort", avgDuration: "50",
                extraMetadata: m => m
                    .With("price_normalny_pln", "14")
                    .With("price_ulgowy_pln", "9")
                    .With("indoor_outdoor", "indoor")
                    .With("building", "Rotunda")
                    .With("pjm_available", "false")
                    .With("last_entry_min_before_close", "45")
                    .With("open_tue_sun", "09:30-17:00")
                    .With("closed_monday", "true")),

            // --- Galeria Sztuki Patriotycznej ---
            // Galeria w fortyfikacjach — malarstwo i rzeźba o tematyce narodowej
            BuildAttraction(
                UuidProductIdentifier.Of("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                "Galeria Sztuki Patriotycznej",
                "Stała ekspozycja malarstwa i rzeźby o tematyce patriotycznej i historycznej, " +
                "eksponowana w bastionach fortu kościuszkowskiego.",
                zone: "fort", avgDuration: "30",
                extraMetadata: m => m
                    .With("price_normalny_pln", "10")
                    .With("price_ulgowy_pln", "7")
                    .With("indoor_outdoor", "indoor")
                    .With("building", "Bastion I")
                    .With("last_entry_min_before_close", "30")
                    .With("open_tue_sun", "09:30-17:00")
                    .With("closed_monday", "true")),

            // --- Wystawa multimedialna „Kościuszko — Człowiek i Symbol" ---
            // Nowoczesna instalacja multimedialna w bastionach fortu
            BuildAttraction(
                UuidProductIdentifier.Of("c3d4e5f6-a7b8-9012-cdef-123456789012"),
                "Wystawa multimedialna „Kościuszko — Człowiek i Symbol"",
                "Nowoczesna ekspozycja multimedialna opowiadająca historię Tadeusza Kościuszki — " +
                "projekcje, interaktywne instalacje, rekonstrukcje bitew.",
                zone: "fort", avgDuration: "40",
                extraMetadata: m => m
                    .With("price_normalny_pln", "18")
                    .With("price_ulgowy_pln", "12")
                    .With("indoor_outdoor", "indoor")
                    .With("building", "Bastion II")
                    .With("interactive", "true")
                    .With("min_age_recommended", "8")
                    .With("last_entry_min_before_close", "40")
                    .With("open_tue_sun", "10:00-17:00")
                    .With("closed_monday", "true")),

            // --- Trasa fortyfikacyjna (obejście fortu) ---
            // Spacer po wałach i bastionach fortu austriackiego z epoki 1850–1856
            BuildAttraction(
                UuidProductIdentifier.Of("d4e5f6a7-b8c9-0123-defa-234567890123"),
                "Trasa Fortyfikacyjna Fortu Kościuszki",
                "Spacerowa trasa po bastionach, wałach i fosie fortu austriackiego z lat 1850–1856. " +
                "Tablice informacyjne omawiają historię fortyfikacji i rolę wzgórza w obronie Krakowa.",
                zone: "fort", avgDuration: "25",
                extraMetadata: m => m
                    .With("price_normalny_pln", "0")
                    .With("price_ulgowy_pln", "0")
                    .With("indoor_outdoor", "outdoor")
                    .With("seasonal", "true")
                    .With("season_start", "1 IV")
                    .With("season_end", "31 X")
                    .With("note", "included_with_kopiec_ticket")),
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
        string avgDuration,
        Func<ProductMetadata, ProductMetadata>? extraMetadata = null)
    {
        var metadata = SeedHelpers.KopiecBase()
            .With("attraction_zone", zone)
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
