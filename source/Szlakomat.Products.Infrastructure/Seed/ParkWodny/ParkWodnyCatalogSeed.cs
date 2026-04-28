using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.CommercialOffer;
using Szlakomat.Products.Domain.Common;

namespace Szlakomat.Products.Infrastructure.Seed.ParkWodny;

/// <summary>
/// Seeds CatalogEntry instances for Park Wodny Kraków products.
/// Aligned with parkwodny.pl (stan na 2025).
/// </summary>
internal static class ParkWodnyCatalogSeed
{
    private static readonly DateOnly SeasonStart = new(2025, 1, 1);
    private static readonly DateOnly SeasonEnd = new(2026, 12, 31);
    private static readonly DateOnly OutdoorPoolStart = new(2025, 5, 1);
    private static readonly DateOnly OutdoorPoolEnd = new(2025, 8, 31);

    public static List<CatalogEntry> Seed(
        ICatalogEntryRepository repo,
        List<ProductType> attractions,
        List<ProductType> services,
        List<PackageType> packages)
    {
        var attraction = attractions.ToDictionary(a => a.Name().Value);
        var service = services.ToDictionary(s => s.Name().Value);
        var package = packages.ToDictionary(p => p.Name().Value);

        var entries = new List<CatalogEntry>();

        // ── Atrakcje (14) ───────────────────────────────────────────────

        entries.Add(BuildEntry(
            attraction["Basen Rekreacyjny"],
            "Basen Rekreacyjny — fala morska i jacuzzi",
            "Duży basen z falą morską, jacuzzi i hydromasażem. Dostępny przez cały rok.",
            new[] { "basen", "aqua-park", "relaks", "rekreacja" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("featured", "true"), ("badge", "bestseller"),
            ("price_1h_normalny_pln", "22"), ("price_1h_ulgowy_pln", "17")));

        entries.Add(BuildEntry(
            attraction["Basen Sportowy"],
            "Basen Sportowy — 6-torowy basen 25 m",
            "6-torowy basen do pływania rekreacyjnego i treningowego. Dostępny przez cały rok.",
            new[] { "basen", "sport", "pływanie" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("price_1h_normalny_pln", "22"), ("price_1h_ulgowy_pln", "17")));

        entries.Add(BuildEntry(
            attraction["Zjeżdżalnie Wodne"],
            "Zjeżdżalnie Wodne — 4 tory",
            "Cztery zjeżdżalnie: rura, wąż, kamikaze i rodzinna. Wliczone w bilet Aqua Park.",
            new[] { "zjeżdżalnie", "aqua-park", "przygoda", "dzieci" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("badge", "hit-sezonu"), ("min_height_solo_cm", "120")));

        entries.Add(BuildEntry(
            attraction["Aqua Kids — Strefa dla Dzieci"],
            "Aqua Kids — strefa wodna dla najmłodszych",
            "Brodzik, fontanny i zjeżdżalnie dla dzieci do 120 cm wzrostu.",
            new[] { "dzieci", "aqua-park", "brodzik", "rodzina" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("badge", "polecane-rodzinom"), ("price_1h_dzieciecy_pln", "14")));

        entries.Add(BuildEntry(
            attraction["Rzeka Leniwca i Jacuzzi"],
            "Rzeka Leniwca i Jacuzzi zewnętrzne",
            "60-metrowa rzeka leniwca i jacuzzi zewnętrzne z podgrzewaną wodą 32°C.",
            new[] { "relaks", "aqua-park", "jacuzzi" },
            Validity.Between(SeasonStart, SeasonEnd)));

        entries.Add(BuildEntry(
            attraction["Strefa Saun"],
            "Strefa Saun — sauna fińska, parowa, solna, infrared (16+)",
            "Kompleks saunowy z 4 typami saun. Wyłącznie dla gości 16+.",
            new[] { "sauna", "spa", "wellness", "relaks" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("badge", "premium"), ("min_age", "16"),
            ("price_normalny_pln", "45"), ("price_combined_aqua_spa_pln", "75")));

        entries.Add(BuildEntry(
            attraction["Basen Zewnętrzny"],
            "Basen Zewnętrzny sezonowy (maj–sierpień)",
            "Odkryty podgrzewany basen z leżakami, dostępny od maja do końca sierpnia.",
            new[] { "basen", "outdoor", "sezonowe", "lato" },
            Validity.Between(OutdoorPoolStart, OutdoorPoolEnd),
            ("seasonal", "true"), ("badge", "lato")));

        entries.Add(BuildEntry(
            attraction["Masaż Klasyczny (50 min)"],
            "Masaż Klasyczny 50 min — SPA",
            "Masaż całego ciała wykonywany przez licencjonowanego fizjoterapeutę. Tylko dla 16+.",
            new[] { "masaż", "spa", "wellness" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("price_pln", "150"), ("advance_booking_h", "24"), ("min_age", "16")));

        entries.Add(BuildEntry(
            attraction["Masaż Gorącymi Kamieniami (60 min)"],
            "Masaż Gorącymi Kamieniami 60 min — SPA",
            "Luksusowy masaż bazaltowymi kamieniami. Tylko dla 16+.",
            new[] { "masaż", "spa", "wellness", "premium" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("badge", "premium"), ("price_pln", "190"), ("min_age", "16")));

        entries.Add(BuildEntry(
            attraction["Aqua Aerobik — zajęcia grupowe"],
            "Aqua Aerobik — zajęcia grupowe",
            "Grupowe zajęcia aqua aerobiku (45 min) w basenie rekreacyjnym. Wymagana rezerwacja.",
            new[] { "ćwiczenia", "aqua-park", "fitness", "zajęcia-grupowe" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("price_per_person_pln", "30"), ("max_participants", "20")));

        entries.Add(BuildEntry(
            attraction["Nauka Pływania — Dzieci (4–12 lat)"],
            "Nauka Pływania — dzieci 4–12 lat",
            "Lekcje pływania dla dzieci prowadzone przez instruktorów PZP. Grupy max. 6 osób.",
            new[] { "nauka-pływania", "dzieci", "instruktor" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("price_individual_pln", "80"), ("price_group_pln", "40")));

        entries.Add(BuildEntry(
            attraction["Nauka Pływania — Dorośli"],
            "Nauka Pływania — dorośli",
            "Indywidualne lekcje pływania dla dorosłych z certyfikowanym instruktorem PZP.",
            new[] { "nauka-pływania", "dorośli", "instruktor" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("price_individual_pln", "100"), ("min_age", "16")));

        entries.Add(BuildEntry(
            service["Wypożyczenie ręcznika"],
            "Wypożyczenie ręcznika kąpielowego",
            "Duży ręcznik kąpielowy na czas wizyty. Kaucja zwrotna.",
            new[] { "usługa", "wyposażenie" },
            Validity.Always(),
            ("price_pln", "10"), ("deposit_pln", "20")));

        entries.Add(BuildEntry(
            service["Parking — bilet jednorazowy"],
            "Parking przy Parku Wodnym",
            "Parking płatny przy ul. Dobrego Pasterza 126, pojemność 200 miejsc.",
            new[] { "usługa", "parking", "logistyka" },
            Validity.Always(),
            ("price_2h_pln", "6"), ("price_4h_pln", "10"), ("price_full_day_pln", "15")));

        // ── Pakiety (8) ─────────────────────────────────────────────────

        entries.Add(BuildEntry(
            package["Bilet Aqua Park"],
            "Bilet Aqua Park — 2h / 3h / całodniowy",
            "Bilet wstępu do strefy Aqua Park. Szafka i opaska w cenie. Czasy: 2h, 3h lub całodniowy.",
            new[] { "pakiet", "bilet", "aqua-park" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("featured", "true"), ("badge", "polecany"),
            ("price_2h_normalny_pln", "35"), ("price_2h_ulgowy_pln", "27"),
            ("price_full_normalny_pln", "55"), ("price_full_ulgowy_pln", "42")));

        entries.Add(BuildEntry(
            package["Bilet Aqua Park + SPA"],
            "Bilet Aqua Park + SPA (16+)",
            "Bilet łączony: pełny Aqua Park + strefa saun. Tylko dla gości 16+.",
            new[] { "pakiet", "bilet", "aqua-park", "spa" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("featured", "true"), ("badge", "bestseller"),
            ("price_normalny_pln", "75"), ("min_age", "16")));

        entries.Add(BuildEntry(
            package["Bilet Rodzinny (2+2)"],
            "Bilet Rodzinny — 2 dorosłych + 2 dzieci",
            "Całodniowy bilet rodzinny dla 2 dorosłych i 2 dzieci do 15 lat. Oszczędność do 25%.",
            new[] { "pakiet", "bilet", "rodzinny", "aqua-park" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("badge", "rodzinne"), ("price_pln", "129")));

        entries.Add(BuildEntry(
            package["Bilet Grupowy (min. 10 osób)"],
            "Bilet Grupowy — min. 10 osób",
            "Bilety grupowe całodniowe dla zorganizowanych grup. Zniżka 20%. Rezerwacja min. 3 dni.",
            new[] { "pakiet", "bilet", "grupowy", "aqua-park" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("min_group_size", "10"), ("price_normalny_pln", "44")));

        entries.Add(BuildEntry(
            package["Bilet Szkoła — Wycieczka Edukacyjna"],
            "Bilet Szkoła — wycieczka edukacyjna",
            "Pakiet szkolny: 2h Aqua Park + opcjonalna lekcja pływania. Nauczyciel gratis (1:15).",
            new[] { "pakiet", "szkoła", "edukacja", "dzieci" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("badge", "edukacyjne"), ("price_uczen_pln", "28"),
            ("teacher_free", "true")));

        entries.Add(BuildEntry(
            package["Karnet Miesięczny — Aqua Park"],
            "Karnet Miesięczny — Aqua Park",
            "30-dniowy imienny karnet na nielimitowane wejścia do strefy Aqua Park.",
            new[] { "pakiet", "karnet", "aqua-park", "miesięczny" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("price_normalny_pln", "199"), ("price_ulgowy_pln", "149"), ("validity_days", "30")));

        entries.Add(BuildEntry(
            package["Karnet Miesięczny — Aqua Park + SPA"],
            "Karnet Miesięczny — Aqua Park + SPA (16+)",
            "30-dniowy imienny karnet na Aqua Park i strefę saun. Tylko dla gości 16+.",
            new[] { "pakiet", "karnet", "aqua-park", "spa", "miesięczny" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("price_normalny_pln", "299"), ("min_age", "16"), ("validity_days", "30")));

        entries.Add(BuildEntry(
            package["Wizyta Wellness — SPA + Masaż"],
            "Wizyta Wellness — SPA + Masaż 50 min (16+)",
            "Pakiet wellness: 3h strefa saun + masaż klasyczny 50 min + ręcznik. Rezerwacja 24h wcześniej.",
            new[] { "pakiet", "wellness", "spa", "masaż", "premium" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("badge", "premium"), ("price_normalny_pln", "185"), ("min_age", "16")));

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
            builder.WithCategory(category);

        foreach (var (key, value) in metadata)
            builder.WithMetadata(key, value);

        return builder.Build();
    }
}
