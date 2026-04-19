using System.Text.RegularExpressions;

namespace Szlakomat.Products.Domain.Common.Identifiers;

/// <summary>
/// GTIN (Global Trade Item Number) - standardowy identyfikator dla produktów detalicznych.
/// Obsługuje wiele struktur danych:
/// - GTIN-8 (EAN-8): 8 cyfr
/// - GTIN-12 (UPC-A): 12 cyfr
/// - GTIN-13 (EAN-13): 13 cyfr
/// - GTIN-14: 14 cyfr
///
/// Wszystkie zawierają prefiks firmy, numer odniesienia towaru i cyfrę kontrolną.
/// </summary>
public record GtinProductIdentifier : IProductIdentifier
{
    public string Value { get; }

    public GtinProductIdentifier(string? value)
    {
        Guard.IsNotNullOrWhiteSpace(value);
        string normalized = Regex.Replace(value, @"[-\s]", "");
        Guard.IsTrue(Regex.IsMatch(normalized, @"^\d{8}$|^\d{12}$|^\d{13}$|^\d{14}$"),
            "GTIN must be 8, 12, 13, or 14 digits");
        if (!IsValidCheckDigit(normalized))
            throw new ArgumentException("Invalid GTIN check digit", nameof(value));
        Value = normalized;
    }

    public static GtinProductIdentifier Of(string value) => new(value);

    public string Type() => $"GTIN-{Value.Length}";

    public override string ToString() => Value;

    private static bool IsValidCheckDigit(string gtin)
    {
        int sum = 0;
        for (int i = 0; i < gtin.Length - 1; i++)
        {
            int digit = int.Parse(gtin[i].ToString());
            // Naprzemiennie mnożymy przez 3 i 1, zaczynając od prawej
            int multiplier = ((gtin.Length - i) % 2 == 0) ? 3 : 1;
            sum += digit * multiplier;
        }
        int checkDigit = int.Parse(gtin[^1].ToString());
        int calculatedCheck = (10 - (sum % 10)) % 10;
        return checkDigit == calculatedCheck;
    }
}
