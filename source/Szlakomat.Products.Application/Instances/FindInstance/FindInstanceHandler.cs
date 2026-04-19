using MediatR;
using Szlakomat.Products.Domain.Instances;
using Szlakomat.Products.Application.Instances.Common;

namespace Szlakomat.Products.Application.Instances.FindInstance;

internal sealed class FindInstanceHandler : IRequestHandler<FindInstanceQuery, InstanceView?>
{
    private readonly IInstanceRepository _repo;

    public FindInstanceHandler(IInstanceRepository repo)
    {
        _repo = repo;
    }

    public Task<InstanceView?> Handle(FindInstanceQuery request, CancellationToken cancellationToken)
    {
        var instance = _repo.FindByStringId(request.InstanceId);
        return Task.FromResult(instance is not null ? ToView(instance) : null);
    }

    private static InstanceView ToView(IInstance instance)
    {
        var instanceType = instance is PackageInstance ? "PACKAGE" : "PRODUCT";
        return new InstanceView(
            instance.Id().ToString(),
            instanceType,
            instance.Product().Id().ToString()!,
            instance.SerialNumber()?.Value,
            instance.BatchId()?.ToString());
    }
}
