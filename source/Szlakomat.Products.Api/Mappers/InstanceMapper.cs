using Szlakomat.Products.Api.Contracts.Instances;
using Szlakomat.Products.Application.Instances.Common;

namespace Szlakomat.Products.Api.Mappers;

internal static class InstanceMapper
{
    internal static InstanceResponse ToResponse(InstanceView v) =>
        new(v.InstanceId, v.InstanceType, v.ProductId, v.SerialNumber, v.BatchId);
}
