namespace Szlakomat.Products.Domain.Instances;

/// <summary>
/// Descriptive name for a Batch (e.g., "Batch 2024-Q1-001", "LOT-20240115-A").
/// </summary>
public record BatchName
{
    public string Value { get; }

    public BatchName(string? value)
    {
        Guard.IsNotNullOrWhiteSpace(value);
        Value = value;
    }

    public static BatchName Of(string value) => new(value);

    public override string ToString() => Value;
}
