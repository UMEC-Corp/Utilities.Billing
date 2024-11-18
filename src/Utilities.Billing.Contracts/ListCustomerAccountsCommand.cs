namespace Utilities.Billing.Contracts
{
    [GenerateSerializer]
    public class ListCustomerAccountsCommand
    {
        [Id(0)]
        public int? Offset { get; set; }
        [Id(1)]
        public int? Limit { get; set; }
    }
}