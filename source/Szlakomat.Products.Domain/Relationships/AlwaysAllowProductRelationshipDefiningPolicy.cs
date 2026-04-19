using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.Relationships;

public class AlwaysAllowProductRelationshipDefiningPolicy : IProductRelationshipDefiningPolicy
{
    public bool CanDefineFor(IProductIdentifier from, IProductIdentifier to, ProductRelationshipType type) => true;
}
