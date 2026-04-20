namespace Szlakomat.Products.Domain.Instances;

/// <summary>
/// Records the status of the delivery of a particular ServiceInstance.
///
/// ServiceInstances are executions of a process, with a lifecycle:
/// SCHEDULED → EXECUTING → COMPLETED/CANCELLED
/// </summary>
public enum ServiceDeliveryStatus
{
    /// <summary>
    /// The ServiceInstance has been scheduled for delivery.
    /// </summary>
    Scheduled,

    /// <summary>
    /// The ServiceInstance is currently in the process of delivery.
    /// </summary>
    Executing,

    /// <summary>
    /// The delivery of the ServiceInstance has been completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// The ServiceInstance has been cancelled before or during execution.
    /// </summary>
    Cancelled
}