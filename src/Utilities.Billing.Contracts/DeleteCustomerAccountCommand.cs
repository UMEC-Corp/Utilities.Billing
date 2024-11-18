namespace Utilities.Billing.Contracts
{
    [GenerateSerializer]
    public class DeleteCustomerAccountCommand
    {
        [Id(0)]
        public string CustomerAccountId { get; set; }
    }
}