using System.Text.RegularExpressions;

namespace Szlakomat.Products.Domain.Common.Identifiers;

/// <summary>
/// ISBN (International Standard Book Number) - standardowy identyfikator dla książek.
/// Format: ISBN-10 (np. 0-201-77060-1)
///
/// Struktura:
/// - Identyfikator grupy (obszar geograficzny/języka)
/// - Identyfikator wydawcy
/// - Identyfikator tytułu
/// - Cyfra kontrolna (0-9 lub X dla 10)
/// </summary>
public record IsbnProductIdentifier : IProductIdentifier
{
    public string Value { get; }

    public IsbnProductIdentifier(string? value)
    {
        Guard.IsNotNullOrWhiteSpace(value);
        // Usuń łączniki i spacje do walidacji
        string normalized = Regex.Replace(value, @"[-\s]", "");
        Guard.IsTrue(Regex.IsMatch(normalized, @"^\d{9}[\dX]$"),
            "ISBN must be 10 digits with optional check digit X");
        if (!IsValidCheckDigit(normalized))
            throw new ArgumentException("Invalid ISBN check digit", nameof(value));
        Value = normalized;
    }

    public static IsbnProductIdentifier Of(string value) => new(value);

    public string Type() => "ISBN";

    public override string ToString() => Value;

    private static bool IsValidCheckDigit(string isbn)
    {
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (10 - i) * int.Parse(isbn[i].ToString());
        }
        char checkChar = isbn[9];
        int checkDigit = (checkChar == 'X') ? 10 : int.Parse(checkChar.ToString());
        return (sum + checkDigit) % 11 == 0;
    }
}
