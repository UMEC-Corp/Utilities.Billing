namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class CreateInvoiceCommand
{
    [Id(0)]
    public string CustomerAccountId { get; set; }
    [Id(1)]
    public string PayerAccount { get; set; }
    [Id(2)]
    public string Amount { get; set; }
}
