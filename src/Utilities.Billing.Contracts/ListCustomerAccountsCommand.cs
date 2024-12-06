namespace Utilities.Billing.Contracts
{
    public class ListCustomerAccountsCommand
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public string TenantId { get; set; }
    }
}