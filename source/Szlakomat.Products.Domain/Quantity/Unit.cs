namespace Szlakomat.Products.Domain.Quantity;

public record Unit(string Symbol, string Name)
{
    public static Unit Pieces() => new("pcs", "Pieces");
    public static Unit Kilograms() => new("kg", "Kilograms");
    public static Unit Liters() => new("l", "Liters");
    public static Unit Meters() => new("m", "Meters");
    public static Unit SquareMeters() => new("m²", "Square Meters");
    public static Unit CubicMeters() => new("m³", "Cubic Meters");
    public static Unit Hours() => new("h", "Hours");
    public static Unit Minutes() => new("min", "Minutes");
    public static Unit Of(string symbol, string name) => new(symbol, name);
}
