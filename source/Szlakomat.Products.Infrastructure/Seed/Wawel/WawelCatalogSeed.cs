using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.CommercialOffer;

namespace Szlakomat.Products.Infrastructure.Seed.Wawel;

/// <summary>
/// Seeds CatalogEntry instances for Wawel Royal Castle products.
/// Aligned with wawel.krakow.pl (stan na 2025).
/// </summary>
internal static class WawelCatalogSeed
{
    private static readonly DateOnly SeasonStart = new(2025, 4, 1);
    private static readonly DateOnly SeasonEnd = new(2026, 12, 31);
    private static readonly DateOnly GardensStart = new(2025, 4, 24);
    private static readonly DateOnly GardensEnd = new(2025, 10, 4);
    private static readonly DateOnly SeasonalAttractionStart = new(2025, 4, 24);
    private static readonly DateOnly SeasonalAttractionEnd = new(2025, 10, 31);
    private static readonly DateOnly CurrentTempExhibitionStart = new(2025, 1, 1);
    private static readonly DateOnly CurrentTempExhibitionEnd = new(2025, 12, 31);
    private static readonly DateOnly PlannedTempExhibitionStart = new(2026, 1, 1);
    private static readonly DateOnly PlannedTempExhibitionEnd = new(2026, 12, 31);

    public static List<CatalogEntry> Seed(
        ICatalogEntryRepository repo,
        List<ProductType> exhibitions,
        List<ProductType> services,
        List<PackageType> packages)
    {
        var exhibition = exhibitions.ToDictionary(e => e.Name().Value);
        var service = services.ToDictionary(s => s.Name().Value);
        var package = packages.ToDictionary(p => p.Name().Value);

        var entries = new List<CatalogEntry>();

        // --- Exhibitions (13) ---
        entries.Add(BuildEntry(
            exhibition["Prywatne Apartamenty Królewskie"],
            "Prywatne Apartamenty Królewskie — Zamek piętro 1",
            "Prywatne apartamenty królewskie z renesansowym wystrojem na I piętrze Zamku, w tym Gabinet Porcelanowy.",
            new[] { "wystawa", "zamek", "wnętrza" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("featured", "true"), ("badge", "bestseller"),
            ("price_standard_pln", "57"), ("price_reduced_pln", "43")));

        entries.Add(BuildEntry(
            exhibition["Reprezentacyjne Komnaty Królewskie"],
            "Reprezentacyjne Komnaty Królewskie — Zamek piętro 2",
            "Reprezentacyjne sale królewskie na II piętrze z kolekcją arrasów wawelskich i Namiotami Tureckimi.",
            new[] { "wystawa", "zamek", "wnętrza", "arrasy" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("featured", "true"),
            ("price_standard_pln", "57"), ("price_reduced_pln", "43")));

        entries.Add(BuildEntry(
            exhibition["Skarbiec Koronny"],
            "Skarbiec Koronny — Insygnia i Regalia",
            "Insygnia koronacyjne, regalia i precjoza królewskie.",
            new[] { "wystawa", "zamek", "skarby" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("badge", "must-see"),
            ("price_standard_pln", "47"), ("price_reduced_pln", "35")));

        entries.Add(BuildEntry(
            exhibition["Zbrojownia"],
            "Zbrojownia — Kolekcja Uzbrojenia XV–XVIII w.",
            "Kolekcja historycznego uzbrojenia i zbroi z XV–XVIII wieku.",
            new[] { "wystawa", "zamek", "militaria" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("price_standard_pln", "47"), ("price_reduced_pln", "35")));

        entries.Add(BuildEntry(
            exhibition["Podziemia Zamku"],
            "Podziemia Zamku — Wawel Zaginiony i Lapidarium",
            "Podziemna trasa archeologiczna z reliktami najstarszych budowli wawelskich. Audioprzewodnik w cenie biletu.",
            new[] { "wystawa", "zamek", "archeologia", "podziemia" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("badge", "unikalne"), ("audio_in_price", "true"),
            ("price_standard_pln", "47"), ("price_reduced_pln", "35")));

        entries.Add(BuildEntry(
            exhibition["Międzymurze. Podziemia Wawelu"],
            "Międzymurze. Podziemia Wawelu",
            "Trasa podziemna i spacerowa między murami obronnymi Wawelu. Audioprzewodnik w cenie biletu.",
            new[] { "wystawa", "zamek", "plener", "mury", "podziemia" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("seasonal", "true"), ("audio_in_price", "true"),
            ("price_standard_pln", "29"), ("price_reduced_pln", "22")));

        entries.Add(BuildEntry(
            exhibition["Ogrody Królewskie"],
            "Ogrody Królewskie",
            "Ogrody królewskie na Wawelu — dostępne sezonowo od końca kwietnia.",
            new[] { "wystawa", "zamek", "plener", "ogrody" },
            Validity.Between(GardensStart, GardensEnd),
            ("seasonal", "true"), ("price_standard_pln", "0")));

        entries.Add(BuildEntry(
            exhibition["Skarby z Łowicza"],
            "Skarby z Łowicza — wystawa czasowa",
            "Aktualna wystawa czasowa prezentująca zbiory pochodzące z Łowicza.",
            new[] { "wystawa", "zamek", "czasowa" },
            Validity.Between(CurrentTempExhibitionStart, CurrentTempExhibitionEnd),
            ("temporary", "true"), ("status", "current"),
            ("price_standard_pln", "5"), ("price_reduced_pln", "3")));

        entries.Add(BuildEntry(
            exhibition["Wawel Odzyskany"],
            "Wawel Odzyskany — budynek nr 7",
            "Ekspozycja w budynku nr 7. Dostępna z biletem na Zamek (piętro 1 lub 2).",
            new[] { "wystawa", "zamek" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("requires_castle_ticket", "true")));

        entries.Add(BuildEntry(
            exhibition["Smocza Jama"],
            "Smocza Jama",
            "Legendarna jaskinia smoka wawelskiego u stóp wzgórza. Atrakcja sezonowa (24 IV – 31 X).",
            new[] { "wystawa", "zamek", "sezonowe", "legendy" },
            Validity.Between(SeasonalAttractionStart, SeasonalAttractionEnd),
            ("seasonal", "true"), ("badge", "ikona-krakowa"),
            ("price_standard_pln", "15"), ("price_reduced_pln", "10")));

        entries.Add(BuildEntry(
            exhibition["Baszta Widokowa"],
            "Baszta Widokowa",
            "Punkt widokowy z panoramą Krakowa. Atrakcja sezonowa (24 IV – 31 X).",
            new[] { "wystawa", "zamek", "sezonowe", "widok" },
            Validity.Between(SeasonalAttractionStart, SeasonalAttractionEnd),
            ("seasonal", "true"),
            ("price_standard_pln", "19"), ("price_reduced_pln", "14")));

        entries.Add(BuildEntry(
            exhibition["Baszta Sandomierska"],
            "Baszta Sandomierska (zamknięta na remont)",
            "Zabytkowa baszta obronna — obecnie zamknięta z powodu remontu konserwatorskiego.",
            new[] { "wystawa", "zamek", "niedostępne" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("status", "closed_for_renovation")));

        entries.Add(BuildEntry(
            exhibition["Przepraszam za bałagan. Michalina Bigaj"],
            "Przepraszam za bałagan. Michalina Bigaj — wystawa czasowa (planowana)",
            "Planowana wystawa czasowa artystki Michaliny Bigaj.",
            new[] { "wystawa", "zamek", "czasowa", "planowana" },
            Validity.Between(PlannedTempExhibitionStart, PlannedTempExhibitionEnd),
            ("temporary", "true"), ("status", "planned")));

        // --- Services (5) ---
        entries.Add(BuildEntry(
            service["Przewodnik licencjonowany — język polski"],
            "Przewodnik licencjonowany po polsku",
            "Usługa przewodnika licencjonowanego w języku polskim.",
            new[] { "usługa", "przewodnik" },
            Validity.Always()));

        entries.Add(BuildEntry(
            service["Przewodnik licencjonowany — język obcy"],
            "Przewodnik licencjonowany — język obcy",
            "Usługa przewodnika licencjonowanego w wybranym języku obcym.",
            new[] { "usługa", "przewodnik", "obcojęzyczny" },
            Validity.Always()));

        entries.Add(BuildEntry(
            service["Audioprzewodnik"],
            "Audioprzewodnik Wawel",
            "Wypożyczenie audioprzewodnika do samodzielnego zwiedzania wystaw.",
            new[] { "usługa", "audio" },
            Validity.Always(),
            ("provider", "Movitech")));

        entries.Add(BuildEntry(
            service["Przechowalnia bagażu"],
            "Przechowalnia bagażu",
            "Obowiązkowa przechowalnia bagażu na dziedzińcu arkadowym.",
            new[] { "usługa", "logistyka" },
            Validity.Always()));

        entries.Add(BuildEntry(
            service["Wypożyczenie słuchawek grupowych"],
            "Słuchawki grupowe",
            "Wypożyczenie zestawów słuchawkowych dla grup powyżej 8 osób.",
            new[] { "usługa", "grupa" },
            Validity.Always()));

        // --- Packages (8) ---
        entries.Add(BuildEntry(
            package["Zamek Królewski na Wawelu — Wizyta indywidualna"],
            "Zamek Królewski na Wawelu — Zwiedzanie Indywidualne",
            "Pakiet zwiedzania indywidualnego: wybór wystaw z opcjonalnym audioprzewodnikiem.",
            new[] { "pakiet", "indywidualny", "zamek" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("featured", "true"), ("badge", "polecany")));

        entries.Add(BuildEntry(
            package["Zamek Królewski na Wawelu — Wycieczka grupowa z przewodnikiem"],
            "Zamek Królewski na Wawelu — Wycieczka Grupowa",
            "Pakiet grupowy z przewodnikiem: wybór wystaw, przewodnik i opcjonalne dodatki.",
            new[] { "pakiet", "grupa", "zamek" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("min_group", "1"), ("max_group", "30")));

        entries.Add(BuildEntry(
            package["Zamek (piętro 1+2)"],
            "Zamek (piętro 1+2) — bilet łączony",
            "Bilet łączony na piętro 1 i 2 Zamku z audioprzewodnikiem w cenie.",
            new[] { "pakiet", "zamek", "bilet-łączony" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("featured", "true"), ("badge", "bestseller"),
            ("price_standard_pln", "95"), ("price_reduced_pln", "71"),
            ("audio_in_price", "true")));

        entries.Add(BuildEntry(
            package["Bilet roczny"],
            "Bilet roczny — Zamek Królewski na Wawelu",
            "Roczny bilet wstępu na wszystkie wystawy stałe.",
            new[] { "pakiet", "zamek", "bilet-roczny" },
            Validity.Always(),
            ("price_standard_pln", "260"), ("price_reduced_pln", "195")));

        entries.Add(BuildEntry(
            package["Wawel dla pasjonatów"],
            "Wawel dla pasjonatów — bilet łączony",
            "Bilet łączony na wiele wystaw dla osób chcących zobaczyć jak najwięcej.",
            new[] { "pakiet", "zamek", "bilet-łączony" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("price_standard_pln", "199"), ("price_reduced_pln", "149")));

        entries.Add(BuildEntry(
            package["Najważniejsze na Wawelu — trasa rodzinna z edukatorem"],
            "Najważniejsze na Wawelu — trasa rodzinna",
            "Trasa rodzinna z edukatorem po najważniejszych punktach Wawelu.",
            new[] { "pakiet", "zamek", "rodzinny", "edukator" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("price_per_person_pln", "39")));

        entries.Add(BuildEntry(
            package["Wawel – najcenniejsze — trasa z przewodnikiem"],
            "Wawel – najcenniejsze — trasa z przewodnikiem",
            "Trasa z licencjonowanym przewodnikiem po najcenniejszych zbiorach Wawelu.",
            new[] { "pakiet", "zamek", "premium", "przewodnik" },
            Validity.Between(SeasonStart, SeasonEnd),
            ("price_standard_pln", "155"), ("price_reduced_pln", "116")));

        entries.Add(BuildEntry(
            package["Szukając Smoka"],
            "Szukając Smoka — trasa sezonowa",
            "Sezonowa trasa łączona: Międzymurze + Smocza Jama (24 IV – 31 X).",
            new[] { "pakiet", "zamek", "sezonowe", "rodzinne", "legendy" },
            Validity.Between(SeasonalAttractionStart, SeasonalAttractionEnd),
            ("seasonal", "true"), ("badge", "rodzinne"),
            ("price_standard_pln", "35"), ("price_reduced_pln", "26")));

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
