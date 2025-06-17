namespace Utilities.Billing.Contracts;

/// <summary>
/// Defines the contract for payment system operations, including wallet management, asset handling, payments, and invoice tracking.
/// </summary>
public interface IPaymentSystem
{
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
    /// <item>Builds a <c>ChangeTrustOperation</c> for the asset.</item>
    /// <item>Constructs a transaction with the change trust operation, signs it with the master key, and submits it.</item>
    /// </list>
    /// </para>
    /// </remarks>
    Task AddAssetAsync(AddStellarAssetCommand command);

    /// <summary>
    /// Adds a payment operation to the Stellar network.
    /// </summary>
    /// <param name="command">The payment command containing payment details.</param>
    /// <remarks>
    /// <para>
    /// This method:
    /// <list type="number">
    /// <item>Sets the network (testnet or public) based on configuration.</item>
    /// <item>Initializes a server instance using the Horizon URL.</item>
    /// <item>Loads the master account and the receiver account from the network.</item>
    /// <item>Creates a non-native asset using the provided asset code and issuer.</item>
    /// <item>Builds a payment operation for the specified amount and asset.</item>
    /// <item>Constructs a transaction with the payment operation, signs it with the master key, and submits it to the network.</item>
    /// </list>
    /// </para>
    /// </remarks>
    Task AddPaymentAsync(AddPaymentCommand command);

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
    /// <item>Builds two payment operations: one from the master to the device, and one from the tenant to the payer.</item>
    /// <item>Serializes the invoice ID as a memo and adds it to the transaction.</item>
    /// <item>Builds the transaction, signs it with the master key, and returns the transaction XDR as a base64 string.</item>
    /// </list>
    /// </para>
    /// </remarks>
    Task<string> CreateInvoiceXdr(CreateInvoiceXdrCommand command);

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
    /// <item>Creates a create account operation to fund the new account.</item>
    /// <item>Creates a set options operation to set thresholds and add the master account as a signer.</item>
    /// <item>Builds a transaction with both operations, signs it with both the master and new key pairs, and submits it.</item>
    /// <item>Returns the new account's public key.</item>
    /// </list>
    /// </para>
    /// </remarks>
    Task<string> CreateWalletAsync(CreateWalletCommand command);

    /// <summary>
    /// Gets the account ID of the master Stellar account.
    /// </summary>
    /// <returns>The master account ID.</returns>
    /// <remarks>
    /// <para>
    /// This method simply derives the account ID from the configured master account's secret seed and returns it.
    /// </para>
    /// </remarks>
    Task<string> GetMasterAccountAsync();

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
    /// <item>For each transaction, checks if the memo is of type "text" and deserializes it as an invoice memo.</item>
    /// <item>If the memo contains a valid invoice ID that matches one of the requested IDs, adds an invoice information object to the result list with the transaction status.</item>
    /// <item>Continues paging through transactions until all requested invoice IDs are found or no more transactions are available.</item>
    /// </list>
    /// </para>
    /// </remarks>
    Task<ICollection<InvoiceInfomation>> GetInvoicesInformationAsync(IEnumerable<long> invoiceIds);

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
    /// <item>Adds a change trust operation to remove the trustline for the asset.</item>
    /// <item>Adds an account merge operation to merge the sender account into the master account.</item>
    /// <item>Builds, signs, and submits the transaction.</item>
    /// </list>
    /// </para>
    /// </remarks>
    Task DeleteCustomerAccountAsync(DeleteCustomerAccount command);
}
