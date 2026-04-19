using System.Text.RegularExpressions;

namespace Szlakomat.Products.Domain.Instances;

/// <summary>
/// VIN (Vehicle Identification Number) - standardowy numer seryjny dla pojazdów.
///
/// Format: 17 znaków (wielkie litery i cyfry, bez I, O, Q aby uniknąć zamieszania)
/// Struktura:
/// - World Manufacturer Identifier (3 znaki)
/// - Vehicle Descriptor Section (6 znaków)
/// - Cyfra kontrolna (1 znak)
/// - Rok modelu (1 znak)
/// - Kod fabryki (1 znak)
/// - Numer sekwencyjny (6 znaków)
///
/// Przykłady: "5YJ3E1EA1JF000001" (Tesla), "1HGBH41JXMN109186" (Honda)
/// </summary>
public record VinSerialNumber : ISerialNumber
{
    public string Value { get; }
    public string Type => "VIN";

    public VinSerialNumber(string? value)
    {
        Guard.IsNotNullOrWhiteSpace(value);
        string normalized = value.ToUpperInvariant().Replace(" ", "").Replace("-", "");
        Guard.HasSizeEqualTo(normalized, 17, "VIN must be exactly 17 characters");
        Guard.IsTrue(Regex.IsMatch(normalized, @"^[A-HJ-NPR-Z0-9]{17}$"),
            "VIN must contain only uppercase letters and digits (excluding I, O, Q)");
        Value = normalized;
    }

    public static VinSerialNumber Of(string value) => new(value);

    public override string ToString() => Value;
}
