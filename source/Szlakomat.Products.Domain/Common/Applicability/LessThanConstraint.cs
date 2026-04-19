namespace Szlakomat.Products.Domain.Common.Applicability;

public sealed record LessThanConstraint(string ParameterName, int Threshold) : IApplicabilityConstraint
{
    public bool IsSatisfiedBy(ApplicabilityContext context)
    {
        var value = context.Get(ParameterName);
        if (value == null) return false;
        try
        {
            return int.Parse(value) < Threshold;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}