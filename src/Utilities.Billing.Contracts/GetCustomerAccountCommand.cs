namespace Utilities.Billing.Contracts;

public class GetCustomerAccountCommand
{
    public string CustomerAccountId { get; set; }
    public string TenantId { get; set; }
}
