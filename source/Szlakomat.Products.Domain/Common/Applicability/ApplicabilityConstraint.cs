namespace Szlakomat.Products.Domain.Common.Applicability;

// Metody fabrykacyjne
public static class ApplicabilityConstraint
{
    public static IApplicabilityConstraint AlwaysTrue() => new AlwaysTrueConstraint();

    public static IApplicabilityConstraint EqualsTo(string parameterName, string expectedValue) =>
        new EqualsConstraint(parameterName, expectedValue);

    public static IApplicabilityConstraint In(string parameterName, IReadOnlySet<string> allowedValues) =>
        new InConstraint(parameterName, allowedValues);

    public static IApplicabilityConstraint In(string parameterName, params string[] allowedValues) =>
        new InConstraint(parameterName, new HashSet<string>(allowedValues));

    public static IApplicabilityConstraint GreaterThan(string parameterName, int threshold) =>
        new GreaterThanConstraint(parameterName, threshold);

    public static IApplicabilityConstraint LessThan(string parameterName, int threshold) =>
        new LessThanConstraint(parameterName, threshold);

    public static IApplicabilityConstraint Between(string parameterName, int min, int max) =>
        new BetweenConstraint(parameterName, min, max);

    public static IApplicabilityConstraint And(params IApplicabilityConstraint[] constraints) =>
        new AndConstraint(constraints.ToList());

    public static IApplicabilityConstraint Or(params IApplicabilityConstraint[] constraints) =>
        new OrConstraint(constraints.ToList());

    public static IApplicabilityConstraint Not(IApplicabilityConstraint constraint) =>
        new NotConstraint(constraint);
}