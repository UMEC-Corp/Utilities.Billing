using Utilities.Common.Data;

namespace Utilities.Billing.Data.Entities;

public class Payment : DbEntityWithLongKey
{
    public Guid AccountId { get; set; }
    public virtual Account Account { get; set; }
    public Guid AssetId { get; set; }
    public Asset Asset { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? Transaction { get; set; }
}