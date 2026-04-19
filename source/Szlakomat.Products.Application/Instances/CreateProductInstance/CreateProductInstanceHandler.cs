using System.Globalization;
using MediatR;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Instances;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Application.Instances.CreateProductInstance;

internal sealed class CreateProductInstanceHandler : IRequestHandler<CreateProductInstance, Result<string, InstanceId>>
{
    private readonly IProductTypeRepository _productTypeRepo;
    private readonly IInstanceRepository _instanceRepo;

    public CreateProductInstanceHandler(IProductTypeRepository productTypeRepo, IInstanceRepository instanceRepo)
    {
        _productTypeRepo = productTypeRepo;
        _instanceRepo = instanceRepo;
    }

    public Task<Result<string, InstanceId>> Handle(CreateProductInstance command, CancellationToken cancellationToken)
    {
        var productType = _productTypeRepo.FindByIdValue(command.ProductTypeId);
        if (productType is null)
            return Task.FromResult(Result<string, InstanceId>.FailureOf($"ProductType not found: {command.ProductTypeId}"));

        if (!decimal.TryParse(command.Quantity, NumberStyles.Number, CultureInfo.InvariantCulture, out var quantityValue))
            return Task.FromResult(Result<string, InstanceId>.FailureOf($"Invalid quantity: '{command.Quantity}'. Expected invariant-culture decimal."));

        var builder = new InstanceBuilder(InstanceId.NewOne());

        if (command.SerialNumber is not null)
            builder.WithSerial(ISerialNumber.Of(command.SerialNumber));

        if (command.BatchId is not null)
            builder.WithBatch(BatchId.Of(command.BatchId));

        var quantity = Quantity.Of(quantityValue, ParseUnit(command.Unit));
        var productBuilder = builder.AsProductInstance(productType).WithQuantity(quantity);

        if (command.Features is not null)
        {
            foreach (var featureConfig in command.Features)
            {
                var featureType = productType.FeatureTypes().GetFeatureType(featureConfig.FeatureName);
                if (featureType is null)
                    return Task.FromResult(Result<string, InstanceId>.FailureOf($"Feature type not found: {featureConfig.FeatureName}"));

                var featureInstance = ProductFeatureInstance.FromString(featureType, featureConfig.Value);
                productBuilder.WithFeature(featureInstance);
            }
        }

        var instance = productBuilder.Build();
        _instanceRepo.Save(instance);
        return Task.FromResult(Result<string, InstanceId>.SuccessOf(instance.Id()));
    }

    private static Szlakomat.Products.Domain.Quantity.Unit ParseUnit(string symbol) => symbol.ToLowerInvariant() switch
    {
        "pcs" or "pieces" => Szlakomat.Products.Domain.Quantity.Unit.Pieces(),
        "kg" or "kilograms" => Szlakomat.Products.Domain.Quantity.Unit.Kilograms(),
        "l" or "liters" => Szlakomat.Products.Domain.Quantity.Unit.Liters(),
        "m" or "meters" => Szlakomat.Products.Domain.Quantity.Unit.Meters(),
        "m²" or "m2" or "square meters" => Szlakomat.Products.Domain.Quantity.Unit.SquareMeters(),
        "m³" or "m3" or "cubic meters" => Szlakomat.Products.Domain.Quantity.Unit.CubicMeters(),
        "h" or "hours" => Szlakomat.Products.Domain.Quantity.Unit.Hours(),
        "min" or "minutes" => Szlakomat.Products.Domain.Quantity.Unit.Minutes(),
        _ => Szlakomat.Products.Domain.Quantity.Unit.Of(symbol, symbol)
    };
}
