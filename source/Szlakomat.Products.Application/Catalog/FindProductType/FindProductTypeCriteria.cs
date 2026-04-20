using MediatR;
using Szlakomat.Products.Application.Catalog.Common;

namespace Szlakomat.Products.Application.Catalog.FindProductType;

/// <summary>
/// Criteria to find a ProductType by its identifier.
/// </summary>
public record FindProductTypeCriteria(
    string ProductId // Will be parsed to ProductIdentifier
) : IRequest<ProductTypeView?>;
