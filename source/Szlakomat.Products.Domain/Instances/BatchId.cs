namespace Szlakomat.Products.Domain.Instances;

/// <summary>
/// Unique identifier for a Batch.
/// </summary>
public record BatchId(Guid Value)
{
    public static BatchId NewOne() => new(Guid.NewGuid());

    public static BatchId Of(string value) => new(Guid.Parse(value));

    public override string ToString() => Value.ToString();
}
