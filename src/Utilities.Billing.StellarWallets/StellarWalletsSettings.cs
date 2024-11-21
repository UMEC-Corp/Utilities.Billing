namespace Utilities.Billing.StellarWallets;

public class StellarWalletsSettings
{
    public static string SectionName = nameof(StellarWalletsSettings);
    public string HorizonUrl { get; set; }
    public string SecretSeed { get; set; }
    public bool UseTestnet { get; set; }
}