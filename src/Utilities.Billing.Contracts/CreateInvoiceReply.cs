namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class CreateInvoiceReply
{
    [Id(0)]
    public string Xdr { get; set; }
}
