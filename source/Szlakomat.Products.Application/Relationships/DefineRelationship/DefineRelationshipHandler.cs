using MediatR;
using Szlakomat.Products.Application.Catalog.Common;
using Szlakomat.Products.Domain.Catalog.ProductType;
using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Relationships;

namespace Szlakomat.Products.Application.Relationships.DefineRelationship;

internal sealed class DefineRelationshipHandler : IRequestHandler<DefineRelationship, Result<string, ProductRelationshipId>>
{
    private readonly ProductRelationshipFactory _factory;
    private readonly IProductRelationshipRepository _repository;
    private readonly IProductTypeRepository _productTypeRepository;

    public DefineRelationshipHandler(
        ProductRelationshipFactory factory,
        IProductRelationshipRepository repository,
        IProductTypeRepository productTypeRepository)
    {
        _factory = factory;
        _repository = repository;
        _productTypeRepository = productTypeRepository;
    }

    public Task<Result<string, ProductRelationshipId>> Handle(DefineRelationship command, CancellationToken cancellationToken)
    {
        var fromIdResult = ProductTypeMapper.TryParseProductIdentifier(command.FromIdentifierType, command.FromProductId);
        if (fromIdResult.IsFailure())
            return Task.FromResult(Result<string, ProductRelationshipId>.FailureOf(fromIdResult.FailureValue));

        var toIdResult = ProductTypeMapper.TryParseProductIdentifier(command.ToIdentifierType, command.ToProductId);
        if (toIdResult.IsFailure())
            return Task.FromResult(Result<string, ProductRelationshipId>.FailureOf(toIdResult.FailureValue));

        var typeResult = TryParseRelationshipType(command.RelationshipType);
        if (typeResult.IsFailure())
            return Task.FromResult(Result<string, ProductRelationshipId>.FailureOf(typeResult.FailureValue));

        var fromId = fromIdResult.SuccessValue;
        var toId = toIdResult.SuccessValue;

        if (_productTypeRepository.FindById(fromId) == null)
            return Task.FromResult(Result<string, ProductRelationshipId>.FailureOf($"From product not found: {command.FromProductId}"));
        if (_productTypeRepository.FindById(toId) == null)
            return Task.FromResult(Result<string, ProductRelationshipId>.FailureOf($"To product not found: {command.ToProductId}"));

        var result = _factory.DefineFor(fromId, toId, typeResult.SuccessValue)
            .PeekSuccess(_repository.Save)
            .Map(rel => rel.Id);
        return Task.FromResult(result);
    }

    private static Result<string, ProductRelationshipType> TryParseRelationshipType(string type) =>
        type?.ToUpper() switch
        {
            "UPGRADABLE_TO" => Result<string, ProductRelationshipType>.SuccessOf(ProductRelationshipType.UpgradableTo),
            "SUBSTITUTED_BY" => Result<string, ProductRelationshipType>.SuccessOf(ProductRelationshipType.SubstitutedBy),
            "REPLACED_BY" => Result<string, ProductRelationshipType>.SuccessOf(ProductRelationshipType.ReplacedBy),
            "COMPLEMENTED_BY" => Result<string, ProductRelationshipType>.SuccessOf(ProductRelationshipType.ComplementedBy),
            "COMPATIBLE_WITH" => Result<string, ProductRelationshipType>.SuccessOf(ProductRelationshipType.CompatibleWith),
            "INCOMPATIBLE_WITH" => Result<string, ProductRelationshipType>.SuccessOf(ProductRelationshipType.IncompatibleWith),
            _ => Result<string, ProductRelationshipType>.FailureOf($"Unknown relationship type: {type}")
        };
}
