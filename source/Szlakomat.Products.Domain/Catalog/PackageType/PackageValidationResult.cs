namespace Szlakomat.Products.Domain.Catalog.PackageType;

/// <summary>
/// Result of package validation - either success or failure with error messages.
/// Used to validate if selected products match package structure rules.
/// </summary>
public record PackageValidationResult(bool Valid, IReadOnlyList<string> Errors)
{
    public static PackageValidationResult Success() => new(true, new List<string>());

    public static PackageValidationResult Failure(string error) => new(false, new List<string> { error });

    public static PackageValidationResult Failure(List<string> errors) => new(false, errors);

    public bool IsValid => Valid;
}
