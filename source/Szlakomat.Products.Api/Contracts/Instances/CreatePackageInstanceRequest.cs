namespace Szlakomat.Products.Api.Contracts.Instances;

public record CreatePackageInstanceRequest(
    string PackageTypeId,
    string? SerialNumber,
    string? BatchId,
    List<SelectedInstanceConfigDto> Selection
);

public record SelectedInstanceConfigDto(string InstanceId, int Quantity);
