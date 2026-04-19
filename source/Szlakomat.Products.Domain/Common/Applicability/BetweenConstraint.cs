namespace Szlakomat.Products.Domain.Common.Applicability;

public sealed record BetweenConstraint(string ParameterName, int Min, int Max) : IApplicabilityConstraint
{
    public bool IsSatisfiedBy(ApplicabilityContext context)
    {
        var value = context.Get(ParameterName);
        if (value == null) return false;
        try
        {
            int numValue = int.Parse(value);
            return numValue >= Min && numValue <= Max;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}