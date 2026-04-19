using MediatR;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Instances;

namespace Szlakomat.Products.Application.Instances.CreateProductInstance;

/// <summary>
/// Command to create a ProductInstance.
/// </summary>
public record CreateProductInstance(
    string ProductTypeId, // Product type this instance belongs to
    string? SerialNumber, // Optional - can be null
    string? BatchId, // Optional - can be null
    string Quantity, // e.g., "1", "5.5", "3.2"
    string Unit, // e.g., "pcs", "kg", "l"
    IReadOnlySet<FeatureInstanceConfig>? Features
) : IRequest<Result<string, InstanceId>>;
