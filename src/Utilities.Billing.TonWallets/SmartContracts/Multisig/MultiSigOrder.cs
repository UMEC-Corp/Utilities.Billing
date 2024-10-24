using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using TonSdk.Core.Boc;
using TonSdk.Core.Crypto;

namespace Utilities.Billing.TonWallets.SmartContracts.Multisig;

public class MultiSigOrder
{
    private Cell _payload;
    private Dictionary<uint, byte[]> _signatures;

    public MultiSigOrder(Cell payload)
    {
        _payload = payload;
        _signatures = new Dictionary<uint, byte[]>();
    }

    public void Sign(uint ownerId, byte[] secretKey)
    {
        _signatures[ownerId] = KeyPair.Sign(_payload, secretKey);
    }

    public void AddSignature(uint ownerId, byte[] signature, MultisigWallet multisigWallet)
    {
        if (!VerifySign(multisigWallet.PublicKeys[ownerId], _payload.Hash.ToBytes(), signature))
        {
            throw new Exception("Invalid signature");
        }
        _signatures[ownerId] = signature;

    }

    //TODO: проверить этот код!
    private bool VerifySign(byte[] publicKey, byte[] signedMessage, byte[] signature)
    {
        var verifier = new Ed25519Signer();
        verifier.Init(false, new Ed25519PublicKeyParameters(publicKey));
        return verifier.VerifySignature(signature);
    }

    public Cell ToCell(uint rootOwnerId)
    {
        var b = new CellBuilder();
        b.StoreBit(false);

        foreach (var pair in _signatures)
        {
            var ownerId = pair.Key;
            var signature = pair.Value;
            b = new CellBuilder()
                .StoreBit(true)
                .StoreRef(
                     new CellBuilder()
                        .StoreBytes(signature)
                        .StoreUInt(ownerId, 8)
                        .StoreCellSlice(b.Build().Parse())
                        .Build()
                );
        }
        return new CellBuilder()
               .StoreUInt(rootOwnerId, 8)
               .StoreCellSlice(b.Build().Parse())
               .StoreCellSlice(_payload.Parse())
               .Build();
    }
}
