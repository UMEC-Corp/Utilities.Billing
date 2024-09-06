namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class GetAssetReply
{
    [Id(0)]
    public Guid Id { get; set; }
    [Id(1)]
    public string Code { get; set; }
    [Id(2)]
    public string IssuerAccount { get; set; }
    [Id(3)]
    public string MasterAccount { get; set; }
    [Id(4)]
    public ICollection<string> ModelCodes { get; set; } = new List<string>();

}
