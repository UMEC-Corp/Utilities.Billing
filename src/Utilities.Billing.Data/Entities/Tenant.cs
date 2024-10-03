using Utilities.Common.Data;

namespace Utilities.Billing.Data.Entities;

public class Tenant : DbEntityWithGuidKey
{
    public string Name { get; set; }
    public string Wallet { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new HashSet<Account>();
    public virtual ICollection<Asset> Assets { get; set; } = new HashSet<Asset>();
}