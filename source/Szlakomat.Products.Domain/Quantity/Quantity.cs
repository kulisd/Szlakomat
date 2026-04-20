namespace Szlakomat.Products.Domain.Quantity;

/// <summary>
/// Reprezentuje ilość czegoś mierzoną w określonej jednostce.
/// </summary>
public record Quantity(decimal Amount, Unit Unit)
{
    public static Quantity Of(decimal amount, Unit unit) => new(amount, unit);
    public static Quantity Of(int amount, Unit unit) => new(amount, unit);

    public override string ToString() => $"{Amount} {Unit.Symbol}";
}
