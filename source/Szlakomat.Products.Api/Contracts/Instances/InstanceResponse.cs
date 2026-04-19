namespace Szlakomat.Products.Api.Contracts.Instances;

public record InstanceResponse(
    string InstanceId,
    string InstanceType,
    string ProductId,
    string? SerialNumber,
    string? BatchId
);
