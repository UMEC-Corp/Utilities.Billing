namespace Utilities.Billing.Data.Entities;

public class AccountType : DbEntity<long>
{
    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
    public string Wallet { get; set; }
    public virtual ICollection<Account> Accounts { get; set; } = new HashSet<Account>();
    public virtual ICollection<ExchangeRate> ExchangeRates { get; set; } = new HashSet<ExchangeRate>();

}