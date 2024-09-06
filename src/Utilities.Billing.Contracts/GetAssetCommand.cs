namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class GetAssetCommand
{
    [Id(0)]
    public string Id { get; set; }
}
