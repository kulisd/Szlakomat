namespace Szlakomat.Products.Domain.Instances;

/// <summary>
/// Selected instance configuration (for PackageInstance).
/// </summary>
public record SelectedInstanceConfig(
    string InstanceId, // ID of ProductInstance or PackageInstance
    int Quantity
);