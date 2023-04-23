namespace Utilities.Billing.Data.Entities;

public class Payment : DbEntity<long>
{
    public long AccountId { get; set; }
    public virtual Account Account { get; set; }
    public decimal Amount { get; set; }
    public DateTime? Date { get; set; }
    public PaymentStatus Status { get; set; }
    public string? Transaction { get; set; }
}