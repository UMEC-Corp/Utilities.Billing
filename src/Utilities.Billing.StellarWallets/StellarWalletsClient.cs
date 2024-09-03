using Microsoft.Extensions.Options;
using StellarDotnetSdk;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Assets;
using StellarDotnetSdk.Operations;
using StellarDotnetSdk.Responses;
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
        UseNetwork();
        var server = new Server(_options.CurrentValue.HorizonUrl);

        var masterKeyPair = KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed);
        var masterAccount = await server.Accounts.Account(masterKeyPair.AccountId);

        var newKeyPair = KeyPair.Random();

        var createAccountOperation = new CreateAccountOperation(newKeyPair, "1", masterKeyPair);

        var setOptionsOperation = new SetOptionsOperation(newKeyPair);
        setOptionsOperation.SetMasterKeyWeight(0);
        setOptionsOperation.SetLowThreshold(1);
        setOptionsOperation.SetMediumThreshold(2);
        setOptionsOperation.SetHighThreshold(3);
        setOptionsOperation.SetSigner(masterKeyPair.AccountId, 4);

        var transaction = new TransactionBuilder(masterAccount)
            .AddOperation(createAccountOperation)
            .AddOperation(setOptionsOperation)
            .Build();

        transaction.Sign(masterKeyPair);
        transaction.Sign(newKeyPair);

        await SendTran(server, transaction);

        return newKeyPair.AccountId;
    }

    public async Task AddAssetAsync(AddStellarAssetCommand command)
    {
        UseNetwork();
        var server = new Server(_options.CurrentValue.HorizonUrl);

        var masterKeyPair = KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed);
        var receiverKeyPair = KeyPair.FromAccountId(command.ReceiverAccountId);
        var receiverAccount = await server.Accounts.Account(receiverKeyPair.AccountId);

        var asset = StellarDotnetSdk.Assets.Asset.CreateNonNativeAsset(command.AssetCode, command.IssuerAccountId);

        var changeTrustOperation = new ChangeTrustOperation(asset);

        var transaction = new TransactionBuilder(receiverAccount).AddOperation(changeTrustOperation).Build();

        transaction.Sign(masterKeyPair);

        await SendTran(server, transaction);
    }

    public async Task<string> GetMasterAccountAsync()
    {
        return KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed).AccountId;
    }

    private void UseNetwork()
    {
        Network.UseTestNetwork();
    }

    private async Task SendTran(Server server, StellarDotnetSdk.Transactions.Transaction transaction)
    {
        var response = await server.SubmitTransaction(transaction);
        if (!response.IsSuccess)
        {
            throw new StellarTransactionFailException(response);
        }

    }

}




[Serializable]
internal class StellarTransactionFailException : Exception
{
    private string _xdr;

    public StellarTransactionFailException()
    {
    }

    public StellarTransactionFailException(SubmitTransactionResponse response)
    {
        _xdr = response.ResultXdr;
    }

    public string GetResultInfo()
    {
        return _xdr;
    }
}



public class StellarWalletsSettings
{
    public string HorizonUrl { get; set; }
    public string SecretSeed { get; set; }
}