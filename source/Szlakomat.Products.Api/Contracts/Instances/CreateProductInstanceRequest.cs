namespace Szlakomat.Products.Api.Contracts.Instances;

public record CreateProductInstanceRequest(
    string ProductTypeId,
    string? SerialNumber,
    string? BatchId,
    string Quantity,
    string Unit,
    List<FeatureInstanceConfigDto>? Features
);

public record FeatureInstanceConfigDto(string FeatureName, string Value);
