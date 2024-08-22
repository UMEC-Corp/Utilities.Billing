using Microsoft.Extensions.Options;
using StellarDotnetSdk;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Assets;
using Utilities.Billing.Contracts;

namespace Utilities.Billing.StellarWallets;
class StellarWalletsClient : IPaymentSystem
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
        var asset = Asset.CreateNonNativeAsset($"UMEC{command.TenantId:N,5}{command.Token}", issuerKeypair.Address);

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
}

public class StellarWalletsSettings
{
    public string HorizonUrl { get; set; }
    public string SecretSeed { get; set; }
    public string MassterAccount { get; set; }
}
