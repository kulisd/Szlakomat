namespace Szlakomat.Products.Domain.Common.Applicability;

public sealed record AndConstraint(List<IApplicabilityConstraint> Constraints) : IApplicabilityConstraint
{
    public bool IsSatisfiedBy(ApplicabilityContext context) =>
        Constraints.All(c => c.IsSatisfiedBy(context));
}