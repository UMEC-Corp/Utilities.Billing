namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class AddInvoicesReply
{
    [Id(0)]
    public List<long> InvoiceIds { get; } = new();
}
