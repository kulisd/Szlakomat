namespace Szlakomat.Products.Domain.Common.Applicability;

public sealed record EqualsConstraint(string ParameterName, string ExpectedValue) : IApplicabilityConstraint
{
    public bool IsSatisfiedBy(ApplicabilityContext context) =>
        context.Get(ParameterName)?.Equals(ExpectedValue) ?? false;
}