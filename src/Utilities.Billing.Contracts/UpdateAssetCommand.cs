namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class UpdateAssetCommand
{
    [Id(0)]
    public string Id { get; set; }
    [Id(1)]
    public ICollection<string> ModelCodes { get; set; } = new List<string>();
}
