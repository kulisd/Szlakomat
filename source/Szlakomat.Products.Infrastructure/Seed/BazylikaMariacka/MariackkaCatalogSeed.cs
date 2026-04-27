using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Infrastructure.Seed.BazylikaMariacka;

/// <summary>
/// Seeds CatalogEntry instances for Bazylika Mariacka products.
/// Two independent routes — each receives a separate catalog entry.
/// Neither route implies or excludes the other; they can be purchased independently.
/// </summary>
internal static class MariackkaCatalogSeed
{
    // Wnętrze: available year-round during church visiting hours
    private static readonly DateOnly InteriorStart = new(2025, 1, 1);
    private static readonly DateOnly InteriorEnd = new(2026, 12, 31);

    // Hejnalica: seasonal from 10 April (defined on the product; here we mirror it)
    private static readonly DateOnly HejnalicaStart = new(2025, 4, 10);
    private static readonly DateOnly HejnalicaEnd = new(2026, 12, 31);

    public static List<CatalogEntry> Seed(
        ICatalogEntryRepository repo,
        ProductType basilikaInterior,
        ProductType hejnalica)
    {
        var entries = new List<CatalogEntry>
        {
            // --- Route 1: Interior ---
            BuildEntry(
                basilikaInterior,
                "Bazylika Mariacka — Zwiedzanie wnętrza",
                "Wejście do gotyckiej Bazyliki Mariackiej (XIV–XV w.) z ołtarzem Wita Stwosza " +
                "i polichromią Jana Matejki. Pon–Sob 11:30–18:00, Ndz i Święta 14:00–18:00.",
                new[] { "bazylika", "kościół", "gotyk", "wnętrze", "kraków", "rynek" },
                Validity.Between(InteriorStart, InteriorEnd),
                ("featured", "true"),
                ("badge", "must-see"),
                ("price_standard_pln", "20"),
                ("price_reduced_pln", "10"),
                ("altarpiece_opens_at", "11:50"),
                ("ticket_office_closes_before_min", "15")),

            // --- Route 2: Hejnalica ---
            BuildEntry(
                hejnalica,
                "Bazylika Mariacka — Hejnalica (Wieża Północna)",
                "Wejście na Wieżę Północną z panoramą Rynku Głównego. Trasa niezależna od " +
                "zwiedzania wnętrza — odrębny bilet. Pt–Ndz 10:10–17:30, wejścia co 10 min. " +
                "Bilety wyłącznie w kasie, brak rezerwacji online.",
                new[] { "bazylika", "wieża", "panorama", "hejnalica", "kraków", "rynek", "sezonowe" },
                Validity.Between(HejnalicaStart, HejnalicaEnd),
                ("featured", "true"),
                ("badge", "widokowe"),
                ("seasonal", "true"),
                ("price_standard_pln", "20"),
                ("price_reduced_pln", "15"),
                ("max_persons_per_slot", "15"),
                ("entry_interval_min", "10"),
                ("online_booking_available", "false"),
                ("route_independent", "true")),
        };

        foreach (var entry in entries)
        {
            repo.Save(entry);
        }

        return entries;
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

        return builder.Build();
    }
}