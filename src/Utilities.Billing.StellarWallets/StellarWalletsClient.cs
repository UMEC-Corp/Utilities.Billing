using Microsoft.Extensions.Options;
using StellarDotnetSdk;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Assets;
using StellarDotnetSdk.Memos;
using StellarDotnetSdk.Operations;
using StellarDotnetSdk.Requests;
using StellarDotnetSdk.Responses;
using StellarDotnetSdk.Transactions;
using StellarDotnetSdk.Xdr;
using System.Globalization;
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
    public async Task AddPaymentAsync(AddPaymentCommand command)
    {
        UseNetwork();
        var server = new Server(_options.CurrentValue.HorizonUrl);

        var masterKeyPair = KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed);
        var masterAccount = await server.Accounts.Account(masterKeyPair.AccountId);

        var recieverKeyPair = KeyPair.FromAccountId(command.RecieverAccountId);
        var recieverAccount = await server.Accounts.Account(recieverKeyPair.AccountId);

        var asset = StellarDotnetSdk.Assets.Asset.CreateNonNativeAsset(command.AssetCode, command.AssetIssuerAccountId);
        var amount = command.Amount.ToString(CultureInfo.InvariantCulture);

        var paymentOperation = new PaymentOperation(recieverKeyPair, asset, amount);

        var transaction = new TransactionBuilder(masterAccount).AddOperation(paymentOperation).Build();

        transaction.Sign(masterKeyPair);

        await SendTran(server, transaction);
    }


    public async Task<string> CreateWalletAsync(CreateWalletCommand command)
    {
        UseNetwork();
        var server = new Server(_options.CurrentValue.HorizonUrl);

        var masterKeyPair = KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed);
        var masterAccount = await server.Accounts.Account(masterKeyPair.AccountId);

        var newKeyPair = KeyPair.Random();

        // Почему 3? 
        //
        // В Стелларе есть понятие минимального баланса. На данный момент он составляте 0.5 XLM
        // Для того чтобы кошелек считался "живым", на нем должно быть два минимальных баланса, т.е. 1 XLM
        // Кроме этого, на увеличение минимального баланса влияют Subentry (0.5 XLM за каждый). В Subentry входят: trustlines , offers, signers, data entries
        // В нашем случае при создании кошелька добавляется Ассет (trustline) (+0.5 XLM), а также дополнительная подпись мастер-аккаунтом (signer) (+0.5 XLM)
        // Получаем минимальный баланс 2 XLM
        // Добавляем еще 1 XLM на комиссиионные расходы при операциях, итого получаем 3 XLM.
        // 
        // Подробнее см. https://developers.stellar.org/docs/learn/fundamentals/stellar-data-structures/accounts#base-reserves-and-subentries
        // и https://developers.stellar.org/docs/learn/fundamentals/lumens#minimum-balance
        var minimalBalance = "3";

        var createAccountOperation = new CreateAccountOperation(newKeyPair, minimalBalance, masterKeyPair);

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

    public async Task<string> CreateInvoiceXdr(CreateInvoiceXdrCommand command)
    {
        UseNetwork();
        var server = new Server(_options.CurrentValue.HorizonUrl);

        var masterKeyPair = KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed);
        var masterAccount = await server.Accounts.Account(masterKeyPair.AccountId);

        var tenantKeyPair = KeyPair.FromAccountId(command.TenantAccountId);
        var tenantAccount = await server.Accounts.Account(tenantKeyPair.AccountId);

        var payerKeyPair = KeyPair.FromAccountId(command.PayerAccountId);
        var payerAccount = await server.Accounts.Account(payerKeyPair.AccountId);

        var deviceKeyPair = KeyPair.FromAccountId(command.DeviceAccountId);
        var deviceAccount = await server.Accounts.Account(deviceKeyPair.AccountId);

        var asset = StellarDotnetSdk.Assets.Asset.CreateNonNativeAsset(command.AssetCode, command.AssetsIssuerAccountId);

        var amount = command.Amount.ToString(CultureInfo.InvariantCulture);
        var devicePaymentOperation = new PaymentOperation(masterKeyPair, asset, amount, deviceAccount.KeyPair);
        var payerPaymentOperation = new PaymentOperation(tenantKeyPair, asset, amount, payerAccount.KeyPair);

        var transaction = new TransactionBuilder(masterAccount)
                .AddOperation(devicePaymentOperation)
                .AddOperation(payerPaymentOperation)
                // A memo allows you to add your own metadata to a transaction. It's
                // optional and does not affect how Stellar treats the transaction.
                .AddMemo(new MemoId((ulong)command.InvoiceId))
                // Wait a maximum of three minutes for the transaction
                //.setTimeout(180)
                // Set the amount of lumens you're willing to pay per operation to submit your transaction
                //.setBaseFee(Transaction.MIN_BASE_FEE)
                .Build();

        transaction.Sign(masterKeyPair);

        return transaction.ToEnvelopeXdrBase64();
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
            throw new StellarTransactionFailException($"Failed to execute transaction. See XDR for details: {response.ResultXdr}");
        }

    }

    public async Task<ICollection<InvoiceInfomation>> GetInvoicesInformationAsync(IEnumerable<long> invoiceIds)
    {
        var result = new List<InvoiceInfomation>();
        var remainingInvoices = invoiceIds.ToList();

        UseNetwork();
        var server = new Server(_options.CurrentValue.HorizonUrl);

        var masterKeyPair = KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed);

        var transactionsPage = await GetPageOfTransactions(server, account: masterKeyPair.AccountId);
        while (transactionsPage.Records.Count > 0 && remainingInvoices.Any())
        {
            foreach (var transaction in transactionsPage.Records)
            {
                if (!long.TryParse(transaction.MemoValue, out var invoiceId) || !remainingInvoices.Contains(invoiceId))
                {
                    continue;
                }

                var item = new InvoiceInfomation
                {
                    Id = invoiceId,
                    Status = transaction.Result.IsSuccess ? PaymentSystemTransactionStatus.Success : PaymentSystemTransactionStatus.Failed
                };

                result.Add(item);

                remainingInvoices.Remove(invoiceId);
            }

            transactionsPage = await transactionsPage.NextPage();
        }


        return result;
    }

    private async Task<StellarDotnetSdk.Responses.Page<TransactionResponse>> GetPageOfTransactions(Server server, int limit = 100, string? account = null, OrderDirection? order = OrderDirection.DESC, string? cursor = null)
    {
        var builder = server.Transactions;

        if (account != null)
        {
            builder.ForAccount(account);
        }

        if (order != null)
        {
            builder.Order(order.Value);
        }

        if (!string.IsNullOrEmpty(cursor))
        {
            builder.Cursor(cursor);
        }

        return await builder.Limit(limit).Execute();
    }


}
