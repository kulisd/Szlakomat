namespace Szlakomat.Products.Domain.Common;

public record ProductName
{
    public string Value { get; }

    public ProductName(string value)
    {
        Guard.IsNotNullOrWhiteSpace(value);
        Value = value;
    }

    public static ProductName Of(string value) => new(value);

    public override string ToString() => Value;
}
