namespace Utilities.Billing.Contracts;

public class AddStellarAssetCommand
{
    public string AssetCode { get; set; }
    public string IssuerAccountId { get; set; }
    public string ReceiverAccountId { get; set; }
}
