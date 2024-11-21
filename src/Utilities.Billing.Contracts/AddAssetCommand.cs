
namespace Utilities.Billing.Contracts;

public class AddAssetCommand
{
    public string AssetCode { get; set; }
    public string Issuer { get; set; }
    public ICollection<string> ModelCodes { get; set; } = new List<string>();
    public string TenantId { get; set; }
}
