namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class AddAssetCommand
{
    [Id(0)]
    public string AssetCode { get; set; }
    [Id(1)]
    public string Issuer { get; set; }
    [Id(2)]
    public ICollection<string> ModelCodes { get; set; } = new List<string>();
}
