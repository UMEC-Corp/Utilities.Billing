using TonSdk.Contracts;
using TonSdk.Core;
using TonSdk.Core.Boc;

namespace Utilities.Billing.TonWallets.SmartContracts.Multisig;

public class MultisigWalletOptions : ContractBaseOptions
{
    public Address? Address { get; set; }
    public Cell? Code { get; set; }
    public int? Workchain { get; set; }

    public uint? WalletId { get; set; }
    public byte RequiredSigners { get; set; }
    public long LastCleaned { get; set; }
    public byte[][] PublicKeys { get; set; } = Array.Empty<byte[]>();
    public byte[][] PendingQueries { get; set; } = Array.Empty<byte[]>();
}