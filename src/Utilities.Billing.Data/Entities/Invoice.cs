namespace Utilities.Billing.Data.Entities;

public class Invoice : DbEntity<long>
{
    public long AccountId { get; set; }
    public virtual Account Account { get; set; }
    public decimal AmountTotal { get; set; }
    public decimal AmountPayed { get; set; }
    public DateTime? Date { get; set; }
}