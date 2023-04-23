namespace Utilities.Billing.Data;

public interface ISoftDelible
{
    DateTime Created { get; set; }
    DateTime? Deleted { get; set; }
}