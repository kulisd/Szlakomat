namespace Szlakomat.Products.Domain.Common.Applicability;

public sealed record InConstraint(string ParameterName, IReadOnlySet<string> AllowedValues) : IApplicabilityConstraint
{
    public bool IsSatisfiedBy(ApplicabilityContext context) =>
        context.Get(ParameterName) is string value && AllowedValues.Contains(value);
}