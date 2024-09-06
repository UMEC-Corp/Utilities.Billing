namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class CreateCustomerAccountReply
{
    [Id(0)]
    public Guid AccountId { get; set; }
}
