using Szlakomat.Products.Domain.Common.Applicability;
using Szlakomat.Products.Domain.Common.Identifiers;

namespace Szlakomat.Products.Domain.Common;

/// <summary>
/// Product - komponent we wzorcu kompozytowym.
/// Może być albo ProductType (liść - zwykły produkt) albo PackageType (kompozyt - pakiet produktów).
/// </summary>
public interface IProduct
{
    IProductIdentifier Id();

    ProductName Name();

    ProductDescription Description();

    ProductMetadata Metadata();

    IApplicabilityConstraint ApplicabilityConstraint();

    bool IsApplicableFor(ApplicabilityContext context) => ApplicabilityConstraint().IsSatisfiedBy(context);
}
