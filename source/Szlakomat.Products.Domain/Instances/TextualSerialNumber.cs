namespace Szlakomat.Products.Domain.Instances;

/// <summary>
/// TextualSerialNumber to prosty numer seryjny oparty na łańcuchu znaków bez specjalnej walidacji.
///
/// Używaj gdy:
/// - Nie jest wymagany format specyficzny dla branży
/// - Masz swój własny wewnętrzny schemat numeracji
/// - Numer seryjny to tylko unikalny łańcuch znaków
///
/// Przykłady: "HYP/2024/00123", "CONS-2024-001", "PKG-2024-XYZ789"
/// </summary>
public record TextualSerialNumber : ISerialNumber
{
    public string Value { get; }
    public string Type => "TEXTUAL";

    public TextualSerialNumber(string? value)
    {
        Guard.IsNotNullOrWhiteSpace(value);
        Value = value;
    }

    public static TextualSerialNumber Of(string value) => new(value);

    public override string ToString() => Value;
}
