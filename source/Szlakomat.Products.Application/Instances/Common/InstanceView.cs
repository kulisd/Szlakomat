namespace Szlakomat.Products.Application.Instances.Common;

public record InstanceView(
    string InstanceId,
    string InstanceType,   // "PRODUCT" | "PACKAGE"
    string ProductId,
    string? SerialNumber,
    string? BatchId
);
