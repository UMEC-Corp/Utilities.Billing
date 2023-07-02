using Utilities.Common.Data;

namespace Utilities.Billing.Data.Entities;

public class AccountType : DbEntityWithLongKey
{
    public string Name { get; set; }
    public string Token { get; set; }
    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; }
}