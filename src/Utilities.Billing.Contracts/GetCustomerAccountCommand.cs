namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class GetCustomerAccountCommand
{
    [Id(0)]
    public string CustomerAccountId { get; set; }
}
