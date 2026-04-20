using MediatR;
using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Instances;

namespace Szlakomat.Products.Application.Instances.CreatePackageInstance;

internal sealed class CreatePackageInstanceHandler : IRequestHandler<CreatePackageInstance, Result<string, InstanceId>>
{
    private readonly IPackageTypeRepository _packageTypeRepo;
    private readonly IInstanceRepository _instanceRepo;

    public CreatePackageInstanceHandler(IPackageTypeRepository packageTypeRepo, IInstanceRepository instanceRepo)
    {
        _packageTypeRepo = packageTypeRepo;
        _instanceRepo = instanceRepo;
    }

    public Task<Result<string, InstanceId>> Handle(CreatePackageInstance command, CancellationToken cancellationToken)
    {
        var packageType = _packageTypeRepo.FindByIdValue(command.PackageTypeId);
        if (packageType is null)
            return Task.FromResult(Result<string, InstanceId>.FailureOf($"PackageType not found: {command.PackageTypeId}"));

        var builder = new InstanceBuilder(InstanceId.NewOne());

        if (command.SerialNumber is not null)
            builder.WithSerial(ISerialNumber.Of(command.SerialNumber));

        if (command.BatchId is not null)
            builder.WithBatch(BatchId.Of(command.BatchId));

        var selection = new List<SelectedInstance>();
        foreach (var selectedConfig in command.Selection)
        {
            var subInstance = _instanceRepo.FindByStringId(selectedConfig.InstanceId);
            if (subInstance is null)
                return Task.FromResult(Result<string, InstanceId>.FailureOf($"Instance not found: {selectedConfig.InstanceId}"));

            selection.Add(new SelectedInstance(subInstance, selectedConfig.Quantity));
        }

        var instance = builder.AsPackageInstance(packageType).WithSelection(selection).Build();
        _instanceRepo.Save(instance);
        return Task.FromResult(Result<string, InstanceId>.SuccessOf(instance.Id()));
    }
}
