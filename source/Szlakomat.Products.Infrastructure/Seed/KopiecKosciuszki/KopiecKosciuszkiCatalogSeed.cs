using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.CommercialOffer;

namespace Szlakomat.Products.Infrastructure.Seed.KopiecKosciuszki;

/// <summary>
/// Seeds CatalogEntry instances for Kopiec Kościuszki products.
/// Aligned with fundacja-kopiec.pl (stan na 2025).
/// </summary>
internal static class KopiecKosciuszkiCatalogSeed
{
    private static readonly DateOnly SeasonStart = new(2025, 4, 1);
    private static readonly DateOnly SeasonEnd = new(2025, 10, 31);
    private static readonly DateOnly YearStart = new(2025, 1, 1);
    private static readonly DateOnly YearEnd = new(2025, 12, 31);

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

        // --- Atrakcje (5) ---
        entries.Add(BuildEntry(
            attraction["Wejście na Kopiec Kościuszki"],
            "Wejście na Kopiec Kościuszki",
            "Spiralna ścieżka na szczyt kopca z panoramą 360° Krakowa i Tatr.",
            new[] { "atrakcja", "widok", "kopiec", "outdoor" },
            Validity.Between(YearStart, YearEnd),
            ("featured", "true"), ("badge", "ikona-krakowa"),
            ("price_normalny_pln", "16"), ("price_ulgowy_pln", "10")));

        entries.Add(BuildEntry(
            attraction["Muzeum im. Tadeusza Kościuszki"],
            "Muzeum im. Tadeusza Kościuszki — ekspozycja stała",
            "Pamiątki, militaria i dokumenty z epoki insurekcji kościuszkowskiej 1794 r.",
            new[] { "muzeum", "historia", "ekspozycja-stała" },
            Validity.Between(YearStart, YearEnd),
            ("featured", "true"),
            ("price_normalny_pln", "14"), ("price_ulgowy_pln", "9")));

        entries.Add(BuildEntry(
            attraction["Galeria Sztuki Patriotycznej"],
            "Galeria Sztuki Patriotycznej",
            "Malarstwo i rzeźba o tematyce patriotycznej w bastionach fortu.",
            new[] { "galeria", "sztuka", "fort" },
            Validity.Between(YearStart, YearEnd),
            ("price_normalny_pln", "10"), ("price_ulgowy_pln", "7")));

        entries.Add(BuildEntry(
            attraction["Wystawa multimedialna „Kościuszko — Człowiek i Symbol""],
            "Wystawa multimedialna „Kościuszko — Człowiek i Symbol"",
            "Interaktywna ekspozycja multimedialna o Tadeuszu Kościuszce — projekcje, instalacje, rekonstrukcje.",
            new[] { "multimedia", "interaktywna", "fort", "historia" },
            Validity.Between(YearStart, YearEnd),
            ("badge", "nowość"), ("interactive", "true"),
            ("price_normalny_pln", "18"), ("price_ulgowy_pln", "12")));

        entries.Add(BuildEntry(
            attraction["Trasa Fortyfikacyjna Fortu Kościuszki"],
            "Trasa Fortyfikacyjna Fortu Kościuszki",
            "Spacer po bastionach i wałach fortu austriackiego (1850–1856). Wliczone w bilet na kopiec.",
            new[] { "trasa", "fort", "outdoor", "sezonowe" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("seasonal", "true"), ("price_normalny_pln", "0")));

        // --- Usługi (4) ---
        entries.Add(BuildEntry(
            service["Przewodnik — język polski"],
            "Przewodnik po Kopcu — język polski",
            "Licencjonowany przewodnik po kopcu i forcie w języku polskim.",
            new[] { "usługa", "przewodnik" },
            Validity.Always()));

        entries.Add(BuildEntry(
            service["Przewodnik — język obcy"],
            "Przewodnik po Kopcu — język obcy",
            "Licencjonowany przewodnik po kopcu i forcie w języku obcym (EN/DE/UA/FR).",
            new[] { "usługa", "przewodnik", "obcojęzyczny" },
            Validity.Always()));

        entries.Add(BuildEntry(
            service["Audioprzewodnik Kopiec Kościuszki"],
            "Audioprzewodnik — Kopiec Kościuszki",
            "Wypożyczenie audioprzewodnika (PL/EN/DE) do samodzielnego zwiedzania.",
            new[] { "usługa", "audio" },
            Validity.Always(),
            ("rental_price_pln", "10")));

        entries.Add(BuildEntry(
            service["Szafka bagażowa"],
            "Szafka bagażowa",
            "Samoobsługowe szafki przy kasie. Kaucja 5 PLN (zwrotna).",
            new[] { "usługa", "logistyka" },
            Validity.Always()));

        // --- Pakiety (6) ---
        entries.Add(BuildEntry(
            package["Bilet wstępu na Kopiec Kościuszki"],
            "Bilet wstępu na Kopiec Kościuszki",
            "Wejście na kopiec + trasa fortyfikacyjna (gratis).",
            new[] { "pakiet", "kopiec", "polecany" },
            Validity.Between(YearStart, YearEnd),
            ("featured", "true"), ("badge", "polecany"),
            ("price_normalny_pln", "16"), ("price_ulgowy_pln", "10")));

        entries.Add(BuildEntry(
            package["Bilet łączony: Kopiec + Muzeum Kościuszki"],
            "Bilet łączony: Kopiec + Muzeum",
            "Kopiec + ekspozycja stała muzeum + trasa fortyfikacyjna.",
            new[] { "pakiet", "bilet-łączony", "kopiec", "muzeum" },
            Validity.Between(YearStart, YearEnd),
            ("featured", "true"), ("badge", "bestseller"),
            ("price_normalny_pln", "25"), ("price_ulgowy_pln", "16")));

        entries.Add(BuildEntry(
            package["Bilet łączony: Kopiec + Wystawa Multimedialna"],
            "Bilet łączony: Kopiec + Multimedia",
            "Kopiec + nowoczesna wystawa multimedialna + trasa fortyfikacyjna.",
            new[] { "pakiet", "bilet-łączony", "kopiec", "multimedia" },
            Validity.Between(YearStart, YearEnd),
            ("price_normalny_pln", "28"), ("price_ulgowy_pln", "18")));

        entries.Add(BuildEntry(
            package["Bilet kompleksowy — Kopiec i Fort"],
            "Bilet kompleksowy — Kopiec i Fort",
            "Pełen bilet: kopiec + muzeum + multimedia + galeria + trasa. Audioprzewodnik w cenie.",
            new[] { "pakiet", "bilet-łączony", "kopiec", "kompleksowy" },
            Validity.Between(YearStart, YearEnd),
            ("badge", "must-see"), ("audio_in_price", "true"),
            ("price_normalny_pln", "45"), ("price_ulgowy_pln", "30")));

        entries.Add(BuildEntry(
            package["Bilet rodzinny — Kopiec i Muzeum"],
            "Bilet rodzinny — Kopiec i Muzeum",
            "2 dorosłych + max 3 dzieci do lat 16. Kopiec + muzeum.",
            new[] { "pakiet", "rodzinny", "kopiec" },
            Validity.Between(YearStart, YearEnd),
            ("badge", "rodzinny"), ("price_pln", "55")));

        entries.Add(BuildEntry(
            package["Wizyta grupowa z przewodnikiem — Kopiec Kościuszki"],
            "Wizyta grupowa z przewodnikiem",
            "Pakiet dla grup 5–25 osób z licencjonowanym przewodnikiem. Rezerwacja min. 7 dni wcześniej.",
            new[] { "pakiet", "grupowy", "kopiec", "przewodnik" },
            Validity.Between(YearStart, YearEnd),
            ("min_group", "5"), ("max_group", "25")));

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
