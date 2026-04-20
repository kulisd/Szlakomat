namespace Szlakomat.Products.Domain.Catalog.ProductType;

/// <summary>
/// Definiuje sposób śledzenia i identyfikowania egzemplarzy produktu w systemie.
///
/// Bezpośrednio odpowiada na pytanie fundamentalne: "Jak rozróżniamy
/// poszczególne egzemplarze tego typu produktu?"
/// </summary>
public enum ProductTrackingStrategy
{
    /// <summary>
    /// UNIKALNY - Produkt jeden w swoim rodzaju.
    /// Przykład: gitara Hetfielda, obraz da Vinciego
    /// </summary>
    Unique,

    /// <summary>
    /// ŚLEDZONY_INDYWIDUALNIE - Każdy egzemplarz unikalnie identyfikowany.
    /// Przykłady: iPhone, umowa hipoteczna, śledzenie paczki
    /// </summary>
    IndividuallyTracked,

    /// <summary>
    /// ŚLEDZONY_PO_PARTIACH - Śledzony przez partię produkcyjną do celów kontroli jakości.
    /// Przykłady: butelki mleka, leki, produkty spożywcze
    /// </summary>
    BatchTracked,

    /// <summary>
    /// ŚLEDZONY_INDYWIDUALNIE_I_PO_PARTIACH - Zarówno indywidualne, jak i śledzenie partii.
    /// Przykłady: telewizory, smartfony (numer seryjny + partia do wycofania)
    /// </summary>
    IndividuallyAndBatchTracked,

    /// <summary>
    /// IDENTYCZNE - Zamienne przedmioty, mogą być lub nie być tworzone jako egzemplarze.
    /// Przykłady: śruby (luzem), worki ryżu (egzemplarz na worek), mąka (luzem)
    /// </summary>
    Identical
}
