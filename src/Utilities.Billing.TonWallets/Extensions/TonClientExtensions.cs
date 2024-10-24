using TonSdk.Client;
using TonSdk.Core;
using Utilities.Billing.TonWallets.SmartContracts.Multisig;

namespace Utilities.Billing.TonWallets.Extensions;

public static class TonClientExtensions
{
    public static async Task<MultisigWallet> GetExistsMultisigWallet(this ITonClient tonClient, string walletAddress)
    {
        var opts = new MultisigWalletOptions();

        var address = new Address(walletAddress);
        opts.Address = address;

        var info = await tonClient.GetAddressInformation(address);
        var data = info.Value.Data;
        var slice = data.Parse();

        //.store_uint(wallet_id, 32)
        //.store_uint(n, 8)
        //.store_uint(k, 8)
        //.store_uint(last_cleaned, 64)
        //.store_dict(owner_infos)
        //.store_dict(pending_queries) 
        opts.WalletId = (uint)slice.LoadUInt(32);
        slice.LoadUInt(8);  // количество владельцев кошелька. Пропускаем.
        opts.RequiredSigners = (byte)slice.LoadUInt(8);
        opts.LastCleaned = (long)slice.LoadUInt(64);

        var ownersDict = slice.ReadDict(MultisigWallet.GetOwnersDictOptions());
        var keys = new byte[ownersDict.Count][];
        for (uint i = 0; i < ownersDict.Count; i++)
        {
            var owKey = ownersDict.Get(i);
            keys[i] = owKey;
        }
        opts.PublicKeys = keys;

        return new MultisigWallet(opts);
    }
}