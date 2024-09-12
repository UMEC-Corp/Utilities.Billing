namespace Utilities.Billing.Contracts;

public class AddPaymentCommand
{
    public string RecieverAccountId { get; set; }
    public string AssetCode { get; set; }
    public string AssetIssuerAccountId { get; set; }
    public decimal Amount { get; set; }
}
