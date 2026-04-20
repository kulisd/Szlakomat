namespace Szlakomat.Products.Domain.Common;

public record ProductDescription
{
    public string Value { get; }

    public ProductDescription(string? value)
    {
        Guard.IsNotNullOrWhiteSpace(value);
        Value = value;
    }

    public static ProductDescription Of(string value) => new(value);

    public override string ToString() => Value;
}
