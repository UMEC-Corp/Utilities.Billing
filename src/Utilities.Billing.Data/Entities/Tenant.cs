using Utilities.Common.Data;

namespace Utilities.Billing.Data.Entities;

public class Tenant : DbEntityWithGuidKey
{
    public string Name { get; set; }
    public string Currency { get; set; }
    public virtual ICollection<AccountHolder> AccountHolders { get; set; } = new HashSet<AccountHolder>();
    public virtual ICollection<AccountType> AccountTypes { get; set; } = new HashSet<AccountType>();
    public string Wallet { get; set; }
}