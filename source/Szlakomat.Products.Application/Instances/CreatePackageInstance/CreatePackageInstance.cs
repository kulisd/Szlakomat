using MediatR;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Instances;

namespace Szlakomat.Products.Application.Instances.CreatePackageInstance;

/// <summary>
/// Command to create a PackageInstance.
/// </summary>
public record CreatePackageInstance(
    string PackageTypeId, // Package type this instance belongs to
    string? SerialNumber, // Optional - can be null
    string? BatchId, // Optional - can be null
    IReadOnlySet<SelectedInstanceConfig> Selection
) : IRequest<Result<string, InstanceId>>;
