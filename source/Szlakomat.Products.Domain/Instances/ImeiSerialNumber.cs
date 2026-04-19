using System.Text.RegularExpressions;

namespace Szlakomat.Products.Domain.Instances;

/// <summary>
/// IMEI (International Mobile Equipment Identity) - standardowy numer seryjny dla urządzeń mobilnych.
///
/// Format: 15 cyfr
/// Struktura:
/// - TAC (Type Allocation Code): 8 cyfr (identyfikuje producenta i model)
/// - SNR (Serial Number): 6 cyfr
/// - CD (Check Digit): 1 cyfra (algorytm Luhna)
///
/// Przykłady: "123456789012345", "490154203237518"
/// </summary>
public record ImeiSerialNumber : ISerialNumber
{
    public string Value { get; }
    public string Type => "IMEI";

    public ImeiSerialNumber(string? value)
    {
        Guard.IsNotNullOrWhiteSpace(value);
        string normalized = Regex.Replace(value, @"[\s-]", "");
        Guard.IsTrue(Regex.IsMatch(normalized, @"^\d{15}$"), "IMEI must be exactly 15 digits");
        if (!IsValidLuhnChecksum(normalized))
            throw new ArgumentException("Invalid IMEI check digit (Luhn algorithm)", nameof(value));
        Value = normalized;
    }

    public static ImeiSerialNumber Of(string value) => new(value);

    public override string ToString() => Value;

    private static bool IsValidLuhnChecksum(string imei)
    {
        int sum = 0;
        bool alternate = false;

        for (int i = imei.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(imei[i].ToString());

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }
}
