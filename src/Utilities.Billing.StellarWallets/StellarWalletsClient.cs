using Microsoft.Extensions.Options;
using StellarDotnetSdk;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Assets;
using StellarDotnetSdk.Operations;
using StellarDotnetSdk.Transactions;
using StellarDotnetSdk.Xdr;
using System.Runtime;
using Utilities.Billing.Contracts;

namespace Utilities.Billing.StellarWallets;
public class StellarWalletsClient : IPaymentSystem
{
    private readonly IOptionsMonitor<StellarWalletsSettings> _options;

    public StellarWalletsClient(IOptionsMonitor<StellarWalletsSettings> options)
    {
        _options = options;
    }
    public Task AddPaymentAsync(AddPaymentCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<string> CreateWalletAsync(CreateWalletCommand command)
    {
        var issuerKeypair = KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed);
        var asset = StellarDotnetSdk.Assets.Asset.CreateNonNativeAsset($"UMEC{command.TenantId:N,5}{command.Token}", issuerKeypair.Address);

        var server = new Server(_options.CurrentValue.HorizonUrl);
        var account = await server.Accounts.WithSigner(issuerKeypair.Address).Account(asset.Code);

        //const transaction = TransactionBuilder.BuildFeeBumpTransaction(account, {
        //        fee: StellarSdk.BASE_FEE,
        //        networkPassphrase: StellarSdk.Networks.TESTNET,
        //    })
        //    // The `changeTrust` operation creates (or alters) a trustline
        //    // The `limit` parameter below is optional
        //    .addOperation(
        //    StellarSdk.Operation.changeTrust({
        //    asset: astroDollar,
        //    limit: "1000",
        //    source: distributorKeypair.publicKey(),
        //}),
        //);

        return null;
    }


    public async Task<string> AddAsset(AddStellarAssetCommand command)
    {
        //Set network and server
        Network.UseTestNetwork();
        var server = new Server(_options.CurrentValue.HorizonUrl);

        //Source keypair from the secret seed
        var receiver = KeyPair.FromAccountId(command.ReceiverAccountId);

        //Load source account data
        var receiverAccountResponse = await server.Accounts.Account(receiver.AccountId);

        //Create source account object
        var receiverAccount = new Account(receiver.AccountId, receiverAccountResponse.SequenceNumber);

        //Create asset object with specific amount
        //You can use native or non native ones.
        var asset = StellarDotnetSdk.Assets.Asset.CreateNonNativeAsset(command.AssetCode, command.IssuerAccountId);

        //Create operation
        var operation = new ChangeTrustOperation(asset);

        //Create transaction and add the payment operation we created
        var transaction = new TransactionBuilder(receiverAccount).AddOperation(operation).Build();

        //Export to Unsigned XDR Base64 (Use this in case you want to sign it in a external signer)
        string unsignedXDR = transaction.ToUnsignedEnvelopeXdrBase64();

        return unsignedXDR;
    }

    public async Task<string> GetMasterAccount()
    {
        return KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed).AccountId;
    }
}



public class StellarWalletsSettings
{
    public string HorizonUrl { get; set; }
    public string SecretSeed { get; set; }
}
