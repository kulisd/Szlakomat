namespace Szlakomat.Products.Domain.Catalog.ProductType;

public static class ProductTrackingStrategyExtensions
{
    /// <summary>
    /// Returns true if each instance must have unique individual identity.
    /// </summary>
    public static bool IsTrackedIndividually(this ProductTrackingStrategy strategy) =>
        strategy is ProductTrackingStrategy.Unique or ProductTrackingStrategy.IndividuallyTracked or ProductTrackingStrategy.IndividuallyAndBatchTracked;

    /// <summary>
    /// Returns true if instances are tracked by production batch.
    /// </summary>
    public static bool IsTrackedByBatch(this ProductTrackingStrategy strategy) =>
        strategy is ProductTrackingStrategy.BatchTracked or ProductTrackingStrategy.IndividuallyAndBatchTracked;

    /// <summary>
    /// Returns true if both individual and batch tracking is required.
    /// </summary>
    public static bool RequiresBothTrackingMethods(this ProductTrackingStrategy strategy) =>
        strategy == ProductTrackingStrategy.IndividuallyAndBatchTracked;

    /// <summary>
    /// Returns true if instances are interchangeable (no unique identity needed).
    /// </summary>
    public static bool IsInterchangeable(this ProductTrackingStrategy strategy) =>
        strategy == ProductTrackingStrategy.Identical;
}
