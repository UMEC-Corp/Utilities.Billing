namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class AddPaymentsReply
{
    [Id(0)]
    public List<long> PaymentIds { get; } = new();
}
