using MediatR;
using Szlakomat.Products.Application.Catalog.Common;
using Szlakomat.Products.Domain.Catalog.PackageType;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Catalog.SelectionRules;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Application.Catalog.DefinePackageType;

internal sealed class DefinePackageTypeHandler : IRequestHandler<DefinePackageType, Result<string, IProductIdentifier>>
{
    private readonly IPackageTypeRepository _repository;

    public DefinePackageTypeHandler(IPackageTypeRepository repository)
    {
        _repository = repository;
    }

    public Task<Result<string, IProductIdentifier>> Handle(DefinePackageType command, CancellationToken cancellationToken)
    {
        if (command.SelectionRules.Count == 0)
            return Task.FromResult(Result<string, IProductIdentifier>.FailureOf(
                "Package must have at least one selection rule"));

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

        var packageBuilder = mainBuilder
            .AsPackageType()
            .WithTrackingStrategy(trackingResult.SuccessValue);

        int setIndex = 0;
        foreach (var cfg in command.SelectionRules)
        {
            var setName = $"set_{setIndex++}";
            switch (cfg)
            {
                case SingleConfig s:
                    packageBuilder.WithSingleChoice(setName, ToIdentifiers(s.ProductIds));
                    break;
                case OptionalConfig o:
                    packageBuilder.WithOptionalChoice(setName, ToIdentifiers(o.ProductIds));
                    break;
                case RequiredConfig r:
                    packageBuilder.WithRequiredChoice(setName, ToIdentifiers(r.ProductIds));
                    break;
                case IsSubsetOfConfig i:
                    packageBuilder.WithChoice(setName, i.Min, i.Max, ToIdentifiers(i.ProductIds));
                    break;
                default:
                    return Task.FromResult(Result<string, IProductIdentifier>.FailureOf(
                        $"Unsupported selection rule config: {cfg.GetType().Name}. " +
                        "Composite rules (And/Or/Not/IfThen) are not yet supported via this command."));
            }
        }

        var packageType = packageBuilder.Build();
        _repository.Save(packageType);
        return Task.FromResult(Result<string, IProductIdentifier>.SuccessOf(packageType.Id()));
    }

    private static IProductIdentifier[] ToIdentifiers(IReadOnlySet<string> productIds) =>
        productIds.Select(id => (IProductIdentifier)UuidProductIdentifier.Of(id)).ToArray();
}
