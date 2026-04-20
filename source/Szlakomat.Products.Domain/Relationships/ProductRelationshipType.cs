namespace Szlakomat.Products.Domain.Relationships;

/// <summary>
/// All relationships are asymmetric (directed): from → to
/// </summary>
public enum ProductRelationshipType
{
    /// <summary>
    /// Produkt A może zostać ulepszony do produktu B (np. iPhone 15 → iPhone 16).
    /// Kierunek: from jest starszą/niższą wersją, to jest nowszą/wyższą.
    /// </summary>
    UpgradableTo,

    /// <summary>
    /// Produkt A może być zastąpiony produktem B jako zamiennik funkcjonalny.
    /// Oba produkty mogą współistnieć w ofercie. Kierunek: from → to (zamiennik).
    /// </summary>
    SubstitutedBy,

    /// <summary>
    /// Produkt A został wycofany i zastąpiony produktem B.
    /// Oznacza koniec życia produktu A. Kierunek: from (wycofany) → to (następca).
    /// </summary>
    ReplacedBy,

    /// <summary>
    /// Produkt A jest uzupełniany przez produkt B (np. telefon → etui ochronne).
    /// Sugeruje cross-sell. Kierunek: from → to (uzupełnienie).
    /// </summary>
    ComplementedBy,

    /// <summary>
    /// Produkt A jest kompatybilny z produktem B (np. ładowarka → laptop).
    /// Informacja o wzajemnej zgodności technicznej. Kierunek: from → to.
    /// </summary>
    CompatibleWith,

    /// <summary>
    /// Produkt A jest niekompatybilny z produktem B.
    /// Ostrzeżenie — produkty nie powinny być używane razem. Kierunek: from → to.
    /// </summary>
    IncompatibleWith
}
