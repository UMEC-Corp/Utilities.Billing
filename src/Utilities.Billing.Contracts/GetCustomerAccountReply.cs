using Utilities.Billing.Data.Entities;

namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class GetCustomerAccountReply
{
    [Id(0)]
    public Guid Id { get; set; }
    [Id(1)]
    public string Wallet { get; set; }
    [Id(2)]
    public Guid AssetId { get; set; }
    [Id(3)]
    public string AssetCode { get; set; }
    [Id(4)]
    public string AssetIssuer { get; set; }
    [Id(5)]
    public string MasterAccount { get; set; }
    public AccountState State { get; set; }
}
