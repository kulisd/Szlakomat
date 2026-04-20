namespace Szlakomat.Products.Domain.Common.Applicability;

public sealed record NotConstraint(IApplicabilityConstraint Constraint) : IApplicabilityConstraint
{
    public bool IsSatisfiedBy(ApplicabilityContext context) =>
        !Constraint.IsSatisfiedBy(context);
}