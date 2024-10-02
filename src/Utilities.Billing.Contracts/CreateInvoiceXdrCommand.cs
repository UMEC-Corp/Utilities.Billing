namespace Utilities.Billing.Contracts;

public class CreateInvoiceXdrCommand
{
    public string PayerAccountId { get; set; }
    public string DeviceAccountId { get; set; }
    public string AssetCode { get; set; }
    public decimal Amount { get; set; }
    public string AssetsIssuerAccountId { get; set; }
    public string TenantAccountId { get; set; }
    public long InvoiceId { get; set; }
}
