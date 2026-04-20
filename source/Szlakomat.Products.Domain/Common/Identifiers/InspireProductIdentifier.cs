using System.Text.RegularExpressions;

namespace Szlakomat.Products.Domain.Common.Identifiers;

/// <summary>
/// Identyfikator produktu oparty na INSPIRE używany w polskich rejestrach dziedzictwa (NID).
/// Format: PL.1.9.ZIPOZ.NID_N_{województwo}_{kategoria}.{numer}
///
/// Przykład: PL.1.9.ZIPOZ.NID_N_12_BK.217616
///   - PL            — kod kraju (Polska)
///   - 1.9           — temat INSPIRE (Protected Sites)
///   - ZIPOZ         — zbiór danych (Zabytki / rejestr dziedzictwa)
///   - NID_N_12_BK   — rejestr NID, województwo 12 (Małopolskie), kategoria BK (budowla/konstrukcja)
///   - 217616        — unikalny numer rejestru
/// </summary>
public record InspireProductIdentifier : IProductIdentifier
{
    private static readonly Regex InspirePattern = new(
        @"^PL\.\d+\.\d+\.\w+\..+$",
        RegexOptions.Compiled);

    public string Value { get; }

    public InspireProductIdentifier(string? value)
    {
        Guard.IsNotNullOrWhiteSpace(value);
        Guard.IsTrue(InspirePattern.IsMatch(value),
            "INSPIRE ID must follow the pattern PL.{theme}.{dataset}.{local_id}, e.g. PL.1.9.ZIPOZ.NID_N_12_BK.217616");
        Value = value;
    }

    public static InspireProductIdentifier Of(string value) => new(value);

    public string Type() => "INSPIRE";

    public override string ToString() => Value;
}
