namespace Utilities.Billing.Contracts;

public class CreateInvoiceCommand
{
    public string CustomerAccountId { get; set; }
    public string PayerAccount { get; set; }
    public string Amount { get; set; }
    public string TenantId { get; set; }
}
