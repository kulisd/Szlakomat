using MediatR;
using Szlakomat.Products.Application.Catalog.Common;
using Szlakomat.Products.Domain.Catalog.Features;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Application.Catalog.DefineProductType;

internal sealed class DefineProductTypeHandler : IRequestHandler<DefineProductType, Result<string, IProductIdentifier>>
{
    private readonly IProductTypeRepository _repository;

    public DefineProductTypeHandler(IProductTypeRepository repository)
    {
        _repository = repository;
    }

    public Task<Result<string, IProductIdentifier>> Handle(DefineProductType command, CancellationToken cancellationToken)
    {
        var identifierResult = ProductTypeMapper.TryParseProductIdentifier(command.ProductIdType, command.ProductId);
        if (identifierResult.IsFailure())
            return Task.FromResult(Result<string, IProductIdentifier>.FailureOf(identifierResult.FailureValue));

        var trackingResult = ProductTypeMapper.TryParseTrackingStrategy(command.TrackingStrategy);
        if (trackingResult.IsFailure())
            return Task.FromResult(Result<string, IProductIdentifier>.FailureOf(trackingResult.FailureValue));

        var mainBuilder = new ProductBuilder(
            identifierResult.SuccessValue,
            ProductName.Of(command.Name),
            ProductDescription.Of(command.Description));

        if (command.Metadata != null)
            mainBuilder.WithMetadata(ProductMetadata.Of(new Dictionary<string, string>(command.Metadata)));

        var typeBuilder = mainBuilder.AsProductType(
            ProductTypeMapper.ParseUnit(command.Unit),
            trackingResult.SuccessValue);

        if (command.MandatoryFeatures != null)
            foreach (var feature in command.MandatoryFeatures)
            {
                var constraint = ProductTypeMapper.TryToConstraint(feature.Constraint);
                if (constraint.IsFailure())
                    return Task.FromResult(Result<string, IProductIdentifier>.FailureOf(constraint.FailureValue));
                typeBuilder = typeBuilder.WithMandatoryFeature(
                    ProductFeatureType.Of(feature.Name, constraint.SuccessValue));
            }

        if (command.OptionalFeatures != null)
            foreach (var feature in command.OptionalFeatures)
            {
                var constraint = ProductTypeMapper.TryToConstraint(feature.Constraint);
                if (constraint.IsFailure())
                    return Task.FromResult(Result<string, IProductIdentifier>.FailureOf(constraint.FailureValue));
                typeBuilder = typeBuilder.WithOptionalFeature(
                    ProductFeatureType.Of(feature.Name, constraint.SuccessValue));
            }

        var productType = typeBuilder.Build();
        _repository.Save(productType);
        return Task.FromResult(Result<string, IProductIdentifier>.SuccessOf(productType.Id()));
    }
}
