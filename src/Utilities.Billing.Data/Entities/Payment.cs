using Utilities.Common.Data;

namespace Utilities.Billing.Data.Entities;

public class Payment : DbEntity<long>
{
    public long AccountId { get; set; }
    public virtual Account Account { get; set; }
    public decimal TokenAmount { get; set; }
    public decimal CurrencyAmount { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? DateTo { get; set; }
    public PaymentStatus Status { get; set; }
    public string? Transaction { get; set; }
}