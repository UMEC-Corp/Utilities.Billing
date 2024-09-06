namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class AddPaymentsForInvoicesCommand
{
    [Id(0)]
    public List<long> InvoiceIds { get; } = new();
}
