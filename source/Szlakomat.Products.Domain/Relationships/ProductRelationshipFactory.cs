using Szlakomat.Products.Domain.Common;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.Relationships;

internal class ProductRelationshipFactory
{
    private static readonly IProductRelationshipDefiningPolicy DefaultPolicy =
        new AlwaysAllowProductRelationshipDefiningPolicy();

    private readonly IProductRelationshipDefiningPolicy _policy;
    private readonly Func<ProductRelationshipId> _idSupplier;

    public ProductRelationshipFactory(IProductRelationshipDefiningPolicy? policy, Func<ProductRelationshipId>? idSupplier)
    {
        _policy = policy ?? DefaultPolicy;
        _idSupplier = idSupplier ?? ProductRelationshipId.Random;
    }

    public ProductRelationshipFactory(Func<ProductRelationshipId> idSupplier)
        : this(null, idSupplier)
    {
    }

    public Result<string, ProductRelationship> DefineFor(
        IProductIdentifier from,
        IProductIdentifier to,
        ProductRelationshipType type)
    {
        if (_policy.CanDefineFor(from, to, type))
        {
            return Result<string, ProductRelationship>.SuccessOf(
                ProductRelationship.Of(_idSupplier(), from, to, type));
        }
        else
        {
            return Result<string, ProductRelationship>.FailureOf("POLICIES_NOT_MET");
        }
    }
}
