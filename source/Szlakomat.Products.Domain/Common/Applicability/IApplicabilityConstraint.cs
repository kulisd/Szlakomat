namespace Szlakomat.Products.Domain.Common.Applicability;

/// <summary>
/// Constraint that can be evaluated against ApplicabilityContext.
/// Supports full composition via AndConstraint, OrConstraint, NotConstraint.
/// </summary>
public interface IApplicabilityConstraint
{
    bool IsSatisfiedBy(ApplicabilityContext context);
}