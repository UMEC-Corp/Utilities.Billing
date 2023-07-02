using Utilities.Common.Data;

namespace Utilities.Billing.Data.Entities;

public class AccountHolder : DbEntityWithLongKey
{
    public string Wallet { get; set; }
    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; }
    public decimal Credit { get; set; }
    public virtual ICollection<Account> Accounts { get; set; } = new HashSet<Account>();
}