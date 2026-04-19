namespace Szlakomat.Products.Domain.Common.Applicability;

public sealed record AlwaysTrueConstraint : IApplicabilityConstraint
{
    public bool IsSatisfiedBy(ApplicabilityContext context) => true;
}