namespace Szlakomat.Products.Domain.Instances;

/// <summary>
/// Unique identifier for an Instance (ProductInstance or PackageInstance).
///
/// Instance needs its own identifier because:
/// - SerialNumber is optional (some instances are tracked only by Batch)
/// - Batch is optional (some instances have only SerialNumber)
/// - At least one of them must exist, but they serve different business purposes
/// - We need a consistent primary key for database persistence regardless of which tracking method is used
/// </summary>
public record InstanceId(Guid Value)
{
    public static InstanceId NewOne() => new(Guid.NewGuid());

    public static InstanceId Of(string value) => new(Guid.Parse(value));

    public override string ToString() => Value.ToString();
}
