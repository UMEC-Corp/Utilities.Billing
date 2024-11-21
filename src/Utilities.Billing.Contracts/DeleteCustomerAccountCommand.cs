namespace Utilities.Billing.Contracts
{
    public class DeleteCustomerAccountCommand
    {
        public string CustomerAccountId { get; set; }
        public string TenantId { get; set; }
    }
}