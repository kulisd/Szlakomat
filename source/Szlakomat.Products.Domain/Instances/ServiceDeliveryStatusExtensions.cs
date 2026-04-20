namespace Szlakomat.Products.Domain.Instances;

public static class ServiceDeliveryStatusExtensions
{
    public static bool IsFinished(this ServiceDeliveryStatus status) =>
        status is ServiceDeliveryStatus.Completed or ServiceDeliveryStatus.Cancelled;

    public static bool IsInProgress(this ServiceDeliveryStatus status) =>
        status == ServiceDeliveryStatus.Executing;

    public static bool CanStart(this ServiceDeliveryStatus status) =>
        status == ServiceDeliveryStatus.Scheduled;

    public static bool CanCancel(this ServiceDeliveryStatus status) =>
        status is ServiceDeliveryStatus.Scheduled or ServiceDeliveryStatus.Executing;
}