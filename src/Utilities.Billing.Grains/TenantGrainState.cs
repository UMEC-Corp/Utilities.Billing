using Utilities.Billing.Data.Entities;

namespace Utilities.Billing.Grains;

[GenerateSerializer]
public class TenantGrainState
{
    [Id(0)]
    public string Name { get; set; }
    [Id(1)]
    public string Wallet { get; set; }
    [Id(3)]
    public WalletType WalletType { get; set; }
}