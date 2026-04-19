namespace Szlakomat.Products.Domain.Common.Applicability;

public sealed record OrConstraint(List<IApplicabilityConstraint> Constraints) : IApplicabilityConstraint
{
    public bool IsSatisfiedBy(ApplicabilityContext context) =>
        Constraints.Any(c => c.IsSatisfiedBy(context));
}