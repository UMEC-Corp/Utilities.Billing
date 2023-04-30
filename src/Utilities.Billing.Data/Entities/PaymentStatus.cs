namespace Utilities.Billing.Data.Entities;

public enum PaymentStatus
{
    /// <summary>
    /// Payment was created in the database but not yet sent to the bc
    /// </summary>
    Initial,
    /// <summary>
    /// Transaction is pending
    /// </summary>
    Pending,
    /// <summary>
    /// Transaction is completed
    /// </summary>
    Completed,
    /// <summary>
    /// Payment is failed for some reason
    /// </summary>
    Failed,
}