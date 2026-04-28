using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.CommercialOffer;

namespace Szlakomat.Products.Infrastructure.Seed.OPN;

/// <summary>
/// Seeds CatalogEntry instances for OPN attractions, trails and packages.
/// </summary>
internal static class OPNCatalogSeed
{
    public static List<CatalogEntry> Seed(
        ICatalogEntryRepository repo,
        List<ProductType> attractions,
        List<ProductType> trails,
        List<PackageType> packages)
    {
        var attraction = attractions.ToDictionary(a => a.Name().Value);
        var trail = trails.ToDictionary(t => t.Name().Value);
        var package = packages.ToDictionary(p => p.Name().Value);

        var entries = new List<CatalogEntry>();

        // --- Paid attractions ---
        entries.Add(BuildEntry(
            attraction["Zamek Kazimierzowski w Ojcowie"],
            "Zamek Kazimierzowski w Ojcowie — XIV-wieczne ruiny",
            "Ruiny warowni Kazimierza Wielkiego z neogotycką wieżą i ekspozycją. Widok na Dolinę Prądnika. Bilet 22/15 zł, wyłącznie gotówka.",
            new[] { "atrakcja", "zamek", "historia" },
            Validity.Always(),
            ("price_standard_pln", "22"), ("price_reduced_pln", "15"),
            ("payment_cash_only", "true"), ("badge", "polecany")));

        entries.Add(BuildEntry(
            attraction["Jaskinia Łokietka"],
            "Jaskinia Łokietka — legendarny azyl króla",
            "Zwiedzanie z przewodnikiem. Stała temp. 7–8°C — wymagana kurtka nawet latem. Śliskie podłoże — stabilne obuwie obowiązkowe.",
            new[] { "atrakcja", "jaskinia", "historia", "przyroda" },
            Validity.Always(),
            ("badge", "must-see"), ("temp_celsius", "7-8"),
            ("guided_only", "true")));

        entries.Add(BuildEntry(
            attraction["Jaskinia Ciemna"],
            "Jaskinia Ciemna — 50 000 lat historii",
            "Obozowisko neandertalczyków. Brak oświetlenia elektrycznego — warto mieć własną latarkę. Strome dojście szlakiem zielonym.",
            new[] { "atrakcja", "jaskinia", "archeologia", "przyroda" },
            Validity.Always(),
            ("badge", "unikalne"), ("no_electric_lighting", "true"),
            ("guided_only", "true")));

        entries.Add(BuildEntry(
            attraction["Zamek w Pieskowej Skale"],
            "Zamek w Pieskowej Skale — renesansowa perła",
            "Muzeum w arkadowym renesansowym zamku. Północna brama OPN. Obok: willa Chopin i stawy (szlak czarny).",
            new[] { "atrakcja", "zamek", "muzeum", "historia" },
            Validity.Always(),
            ("badge", "featured")));

        // --- Free attractions ---
        entries.Add(BuildEntry(
            attraction["Brama Krakowska, Jonaszówka i Źródło Miłości"],
            "Brama Krakowska i Jonaszówka",
            "Skalna brama 15 m i krasowe Źródło Miłości. Punkt widokowy Jonaszówka. Płaski teren — jedyne miejsce w OPN dostępne dla wózków i wózków inwalidzkich.",
            new[] { "atrakcja", "przyroda", "widok", "dostępne" },
            Validity.Always(),
            ("price_standard_pln", "0"), ("prams_accessible", "true"),
            ("badge", "rodzinne")));

        entries.Add(BuildEntry(
            attraction["Maczuga Herkulesa"],
            "Maczuga Herkulesa — 25-metrowy ostaniec",
            "Naturalna wizytówka geologiczna OPN. Bezpłatna, widoczna ze szlaku czarnego.",
            new[] { "atrakcja", "przyroda", "geologia" },
            Validity.Always(),
            ("price_standard_pln", "0")));

        entries.Add(BuildEntry(
            attraction["Kaplica na Wodzie"],
            "Kaplica na Wodzie — unikalna konstrukcja na palach",
            "Drewniana kaplica na palach nad rzeką Prądnik. Widoczna ze szlaku czerwonego.",
            new[] { "atrakcja", "sakralne", "architektura" },
            Validity.Always(),
            ("price_standard_pln", "0")));

        entries.Add(BuildEntry(
            attraction["Grodzisko — Kompleks Salomei"],
            "Grodzisko — cisza z dala od tłumów",
            "Kompleks z rzeźbą słonia i pamiątkami po bł. Salomei. Idealne na spokojny spacer poza głównym szlakiem.",
            new[] { "atrakcja", "sakralne", "cisza" },
            Validity.Always(),
            ("price_standard_pln", "0")));

        entries.Add(BuildEntry(
            attraction["Młyn Boronia"],
            "Młyn Boronia — zabytek techniki",
            "Najlepiej zachowany przemysłowy zabytek OPN. Wnętrza tylko po umówieniu telefonicznym.",
            new[] { "atrakcja", "technika", "historia" },
            Validity.Always(),
            ("interior_by_appointment_only", "true")));

        // --- Trails ---
        entries.Add(BuildEntry(
            trail["Szlak Orlich Gniazd (czerwony)"],
            "Szlak Orlich Gniazd (czerwony) — 13,6 km",
            "Główna oś OPN, asfalt dnem doliny. Jedyny szlak dla wózków i wózków inwalidzkich na odcinku Ojców–Brama Krakowska.",
            new[] { "szlak", "łatwy", "dostępne" },
            Validity.Always(),
            ("length_km", "13.6"), ("wheelchair_accessible", "true"),
            ("badge", "polecany")));

        entries.Add(BuildEntry(
            trail["Szlak Warowni Jurajskich (niebieski)"],
            "Szlak Warowni Jurajskich (niebieski) — Wąwóz Ciasne Skałki",
            "Łączy Jaskinię Łokietka z Bramą Krakowską przez wąwóz. Strome — wymagane obuwie z profilowanym bieżnikiem.",
            new[] { "szlak", "średni", "wąwóz" },
            Validity.Always(),
            ("steep", "true"), ("profiled_footwear_required", "true")));

        entries.Add(BuildEntry(
            trail["Szlak Park Zamkowy – Jaskinia Ciemna (zielony)"],
            "Szlak Park Zamkowy – Jaskinia Ciemna (zielony) — górski",
            "Widoki z Góry Koronnej. Charakter górski, strome podejścia. Nie dla osób z lękiem wysokości ani wózków.",
            new[] { "szlak", "trudny", "widoki", "górski" },
            Validity.Always(),
            ("mountain_character", "true"), ("not_recommended_strollers", "true")));

        entries.Add(BuildEntry(
            trail["Szlak Dolina Sąspowska (żółty)"],
            "Szlak Dolina Sąspowska (żółty) — z dala od tłumów",
            "Najspokojniejsza trasa OPN przez Dolinę Sąspowską. Teren podmokły — obuwie wodoodporne wskazane.",
            new[] { "szlak", "łatwy-średni", "cisza" },
            Validity.Always(),
            ("terrain", "podmokły"), ("crowd_avoidance", "true")));

        entries.Add(BuildEntry(
            trail["Szlak Łącznik Widokowy (czarny)"],
            "Szlak Łącznik Widokowy (czarny) — panorama Ojcowa",
            "Łącznik widokowy. Skała Jonaszówka — najlepsze miejsce na panoramiczne zdjęcia Ojcowa.",
            new[] { "szlak", "średni", "widoki" },
            Validity.Always(),
            ("key_viewpoint", "Skała Jonaszówka")));

        // --- Packages ---
        entries.Add(BuildEntry(
            package["OPN — Jaskinie"],
            "OPN — Jaskinie (Łokietka + Ciemna)",
            "Pakiet jaskiniowy: jedna lub obie jaskinie OPN. Pamiętaj o ciepłej kurtce i solidnym obuwiu.",
            new[] { "pakiet", "jaskinia" },
            Validity.Always()));

        entries.Add(BuildEntry(
            package["OPN — Zamki Jurajskie"],
            "OPN — Zamki Jurajskie (bilet łączony)",
            "Bilet łączony na zamki w Ojcowie i Pieskowej Skale — dwa oblicza jurajskiej architektury obronnej.",
            new[] { "pakiet", "zamek", "bilet-łączony" },
            Validity.Always(),
            ("badge", "polecany")));

        entries.Add(BuildEntry(
            package["OPN — Odkryj Park"],
            "OPN — Odkryj Park (pakiet indywidualny)",
            "Wybierz 1–4 płatnych atrakcji OPN według własnego planu wycieczki.",
            new[] { "pakiet", "indywidualny" },
            Validity.Always()));

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
