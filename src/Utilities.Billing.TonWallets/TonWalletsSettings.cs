namespace Utilities.Billing.TonWallets;

public class TonWalletsSettings
{
    public static string SectionName = nameof(TonWalletsSettings);
    public string MasterSecret { get; set; }
    public string Endpoint { get; set; }
    public string ApiKey { get; set; }
}