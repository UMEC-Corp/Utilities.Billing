using Microsoft.Extensions.Options;
using StellarDotnetSdk;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Memos;
using StellarDotnetSdk.Operations;
using StellarDotnetSdk.Requests;
using StellarDotnetSdk.Responses;
using StellarDotnetSdk.Transactions;
using System.Globalization;
using System.Text.Json;
using Utilities.Billing.Contracts;
using Asset = StellarDotnetSdk.Assets.Asset;

namespace Utilities.Billing.StellarWallets;
/// <summary>
/// Provides methods for interacting with Stellar wallets, including wallet creation, asset management, payments, and invoice tracking.
/// </summary>
public class StellarWalletsClient : IPaymentSystem
{
    private readonly IOptionsMonitor<StellarWalletsSettings> _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="StellarWalletsClient"/> class.
    /// </summary>
    /// <param name="options">The options monitor for Stellar wallet settings.</param>
    /// <remarks>
    /// Stores the provided <paramref name="options"/> for use in all Stellar network operations.
    /// </remarks>
    public StellarWalletsClient(IOptionsMonitor<StellarWalletsSettings> options)
    {
        _options = options;
    }

    /// <summary>
    /// Adds a payment operation to the Stellar network.
    /// </summary>
    /// <param name="command">The payment command containing payment details.</param>
    /// <remarks>
    /// <para>
    /// This method:
    /// <list type="number">
    /// <item>Sets the network (testnet or public) based on configuration.</item>
    /// <item>Initializes a <see cref="Server"/> instance using the Horizon URL.</item>
    /// <item>Loads the master account and the receiver account from the network.</item>
    /// <item>Creates a non-native asset using the provided asset code and issuer.</item>
    /// <item>Builds a <see cref="PaymentOperation"/> for the specified amount and asset.</item>
    /// <item>Constructs a transaction with the payment operation, signs it with the master key, and submits it to the network.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public async Task AddPaymentAsync(AddPaymentCommand command)
    {
        UseNetwork();
        var server = new Server(_options.CurrentValue.HorizonUrl);

        var masterKeyPair = KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed);
        var masterAccount = await server.Accounts.Account(masterKeyPair.AccountId);

        var recieverKeyPair = KeyPair.FromAccountId(command.RecieverAccountId);
        var recieverAccount = await server.Accounts.Account(recieverKeyPair.AccountId);

        var asset = Asset.CreateNonNativeAsset(command.AssetCode, command.AssetIssuerAccountId);
        var amount = command.Amount.ToString(CultureInfo.InvariantCulture);

        var paymentOperation = new PaymentOperation(recieverKeyPair, asset, amount);

        var transaction = new TransactionBuilder(masterAccount).AddOperation(paymentOperation).Build();

        transaction.Sign(masterKeyPair);

        await SendTran(server, transaction);
    }

    /// <summary>
    /// Deletes a customer account from the Stellar network, merging its balance and removing trustlines.
    /// </summary>
    /// <param name="command">The command containing account and asset details for deletion.</param>
    /// <remarks>
    /// <para>
    /// This method:
    /// <list type="number">
    /// <item>Sets the network and initializes the server.</item>
    /// <item>Loads the master and sender accounts.</item>
    /// <item>Checks the sender's balances for the specified asset.</item>
    /// <item>If a balance exists and is greater than zero, creates a payment operation to transfer the asset to the master account.</item>
    /// <item>Adds a <see cref="ChangeTrustOperation"/> to remove the trustline for the asset.</item>
    /// <item>Adds an <see cref="AccountMergeOperation"/> to merge the sender account into the master account.</item>
    /// <item>Builds, signs, and submits the transaction.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public async Task DeleteCustomerAccountAsync(DeleteCustomerAccount command)
    {
        UseNetwork();
        var server = new Server(_options.CurrentValue.HorizonUrl);

        var masterKeyPair = KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed);
        var masterAccount = await server.Accounts.Account(masterKeyPair.AccountId);

        var senderKeyPair = KeyPair.FromAccountId(command.CustomerAccountId);
        var senderAccount = await server.Accounts.Account(senderKeyPair.AccountId);
        var balances = senderAccount.Balances;

        var transactionBuilder = new TransactionBuilder(senderAccount);

        var balance = balances.FirstOrDefault(x => x.AssetCode == command.AssetCode && x.AssetIssuer == command.AssetIssuerAccountId);
        if (balance != null)
        {
            var asset = Asset.CreateNonNativeAsset(command.AssetCode, command.AssetIssuerAccountId);
            var amount = balance.BalanceString;
            if (double.TryParse(amount, CultureInfo.InvariantCulture, out var amountNum) && amountNum > 0)
            {
                var paymentOperation = new PaymentOperation(masterKeyPair, asset, amount);
                transactionBuilder.AddOperation(paymentOperation);
            }

            var changeTrustOperation = new ChangeTrustOperation(asset, "0");
            transactionBuilder.AddOperation(changeTrustOperation);
        }

        var accountMergeOperation = new AccountMergeOperation(masterKeyPair);
        var transaction = transactionBuilder
            .AddOperation(accountMergeOperation)
            .Build();

        transaction.Sign(masterKeyPair);

        await SendTran(server, transaction);
    }

    /// <summary>
    /// Creates a new Stellar wallet and sets up account options.
    /// </summary>
    /// <param name="command">The command containing wallet creation options.</param>
    /// <returns>The account ID of the newly created wallet.</returns>
    /// <remarks>
    /// <para>
    /// This method:
    /// <list type="number">
    /// <item>Sets the network and initializes the server.</item>
    /// <item>Loads the master account and generates a new key pair for the wallet.</item>
    /// <item>Calculates the minimal balance required for the new account (see code comments for details).</item>
    /// <item>Creates a <see cref="CreateAccountOperation"/> to fund the new account.</item>
    /// <item>Creates a <see cref="SetOptionsOperation"/> to set thresholds and add the master account as a signer.</item>
    /// <item>Builds a transaction with both operations, signs it with both the master and new key pairs, and submits it.</item>
    /// <item>Returns the new account's public key.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public async Task<string> CreateWalletAsync(CreateWalletCommand command)
    {
        UseNetwork();
        var server = new Server(_options.CurrentValue.HorizonUrl);

        var masterKeyPair = KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed);
        var masterAccount = await server.Accounts.Account(masterKeyPair.AccountId);

        var newKeyPair = KeyPair.Random();

        // See comments in code for minimal balance explanation.
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

    /// <summary>
    /// Adds a trustline for a specified asset to a Stellar account.
    /// </summary>
    /// <param name="command">The command containing asset and receiver account details.</param>
    /// <remarks>
    /// <para>
    /// This method:
    /// <list type="number">
    /// <item>Sets the network and initializes the server.</item>
    /// <item>Loads the master and receiver accounts.</item>
    /// <item>Creates a non-native asset using the provided asset code and issuer.</item>
    /// <item>Builds a <see cref="ChangeTrustOperation"/> for the asset.</item>
    /// <item>Constructs a transaction with the change trust operation, signs it with the master key, and submits it.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public async Task AddAssetAsync(AddStellarAssetCommand command)
    {
        UseNetwork();
        var server = new Server(_options.CurrentValue.HorizonUrl);

        var masterKeyPair = KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed);
        var receiverKeyPair = KeyPair.FromAccountId(command.ReceiverAccountId);
        var receiverAccount = await server.Accounts.Account(receiverKeyPair.AccountId);

        var asset = Asset.CreateNonNativeAsset(command.AssetCode, command.IssuerAccountId);

        var changeTrustOperation = new ChangeTrustOperation(asset);

        var transaction = new TransactionBuilder(receiverAccount).AddOperation(changeTrustOperation).Build();

        transaction.Sign(masterKeyPair);

        await SendTran(server, transaction);
    }

    /// <summary>
    /// Creates a transaction XDR for an invoice payment.
    /// </summary>
    /// <param name="command">The command containing invoice and payment details.</param>
    /// <returns>The base64-encoded XDR string of the transaction.</returns>
    /// <remarks>
    /// <para>
    /// This method:
    /// <list type="number">
    /// <item>Sets the network and initializes the server.</item>
    /// <item>Loads the master, tenant, payer, and device accounts.</item>
    /// <item>Creates a non-native asset for the invoice.</item>
    /// <item>Builds two <see cref="PaymentOperation"/>s: one from the master to the device, and one from the tenant to the payer.</item>
    /// <item>Serializes the invoice ID as a memo and adds it to the transaction.</item>
    /// <item>Builds the transaction, signs it with the master key, and returns the transaction XDR as a base64 string.</item>
    /// </list>
    /// </para>
    /// </remarks>
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

        var invoiceMemo = JsonSerializer.Serialize(new InvoiceMemo { InvoiceId = command.InvoiceId });
        var transaction = new TransactionBuilder(deviceAccount)
                .AddOperation(devicePaymentOperation)
                .AddOperation(payerPaymentOperation)
                .AddMemo(new MemoText(invoiceMemo))
                .Build();

        transaction.Sign(masterKeyPair);

        return transaction.ToEnvelopeXdrBase64();
    }

    /// <summary>
    /// Gets the account ID of the master Stellar account.
    /// </summary>
    /// <returns>The master account ID.</returns>
    /// <remarks>
    /// <para>
    /// This method simply derives the account ID from the configured master account's secret seed and returns it.
    /// </para>
    /// </remarks>
    public async Task<string> GetMasterAccountAsync()
    {
        return KeyPair.FromSecretSeed(_options.CurrentValue.SecretSeed).AccountId;
    }

    /// <summary>
    /// Sets the Stellar network to use (testnet or public) based on configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Checks the <see cref="StellarWalletsSettings.UseTestnet"/> property and calls the appropriate method on <see cref="Network"/> to set the network context for all subsequent operations.
    /// </para>
    /// </remarks>
    private void UseNetwork()
    {
        if (_options.CurrentValue.UseTestnet)
        {
            Network.UseTestNetwork();
        }
        else
        {
            Network.UsePublicNetwork();
        }
    }

    /// <summary>
    /// Submits a transaction to the Stellar network and throws an exception if it fails.
    /// </summary>
    /// <param name="server">The Stellar server instance.</param>
    /// <param name="transaction">The transaction to submit.</param>
    /// <remarks>
    /// <para>
    /// This method submits the provided transaction to the network using the <paramref name="server"/>. If the transaction fails, it throws a <see cref="StellarTransactionFailException"/> with the result XDR for debugging.
    /// </para>
    /// </remarks>
    private async Task SendTran(Server server, StellarDotnetSdk.Transactions.Transaction transaction)
    {
        var response = await server.SubmitTransaction(transaction);
        if (!response.IsSuccess)
        {
            throw new StellarTransactionFailException($"Failed to execute transaction. See XDR for details: {response.ResultXdr}");
        }
    }

    /// <summary>
    /// Retrieves information about invoices by their IDs from the Stellar network.
    /// </summary>
    /// <param name="invoiceIds">The collection of invoice IDs to query.</param>
    /// <returns>A collection of invoice information objects.</returns>
    /// <remarks>
    /// <para>
    /// This method:
    /// <list type="number">
    /// <item>Sets the network and initializes the server.</item>
    /// <item>Loads the master account and retrieves pages of transactions for that account.</item>
    /// <item>For each transaction, checks if the memo is of type "text" and deserializes it as an <see cref="InvoiceMemo"/>.</item>
    /// <item>If the memo contains a valid invoice ID that matches one of the requested IDs, adds an <see cref="InvoiceInfomation"/> object to the result list with the transaction status.</item>
    /// <item>Continues paging through transactions until all requested invoice IDs are found or no more transactions are available.</item>
    /// </list>
    /// </para>
    /// </remarks>
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
                if (transaction.MemoType != "text")
                {
                    continue;
                }

                var memo = JsonSerializer.Deserialize<InvoiceMemo>(transaction.MemoValue);
                if (memo == null || memo.InvoiceId == 0)
                {
                    continue;
                }

                var invoiceId = memo.InvoiceId;

                if (!remainingInvoices.Contains(invoiceId))
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

    /// <summary>
    /// Retrieves a page of transactions for a given account from the Stellar network.
    /// </summary>
    /// <param name="server">The Stellar server instance.</param>
    /// <param name="limit">The maximum number of transactions to retrieve.</param>
    /// <param name="account">The account ID to filter transactions for.</param>
    /// <param name="order">The order direction for transactions.</param>
    /// <param name="cursor">The paging cursor.</param>
    /// <returns>A page of transaction responses.</returns>
    /// <remarks>
    /// <para>
    /// This method builds a transaction query using the provided parameters, including account, order, and cursor, and executes it to retrieve a page of transactions from the Horizon server.
    /// </para>
    /// </remarks>
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
