using Utilities.Billing.Contracts;

namespace Utilities.Billing.Api.Services;

/// <summary>
/// Provides tenant-related operations, including tenant management, asset management, customer account management, and invoice processing.
/// </summary>
public interface ITenantService
{
    /// <summary>
    /// Adds a new asset for a tenant.
    /// </summary>
    /// <param name="command">The command containing asset details.</param>
    /// <returns>The reply containing the new asset's ID.</returns>
    /// <remarks>
    /// Checks if the asset already exists for the tenant. If not, adds a trustline for the asset using the payment system, creates the asset entity, and associates equipment models with it. Saves all changes to the database and returns the asset's ID.
    /// </remarks>
    Task<AddAssetReply> AddAsset(AddAssetCommand command);

    /// <summary>
    /// Retrieves asset details for a tenant.
    /// </summary>
    /// <param name="command">The command containing asset and tenant IDs.</param>
    /// <returns>The asset reply with asset details.</returns>
    /// <remarks>
    /// Finds the asset by ID and verifies tenant ownership. If found, returns asset details including associated model codes and master account information. Throws an error if not found or if the asset belongs to another tenant.
    /// </remarks>
    Task<GetAssetReply> GetAsset(GetAssetCommand command);

    /// <summary>
    /// Updates an asset and its associated equipment models.
    /// </summary>
    /// <param name="command">The command containing updated asset and model codes.</param>
    /// <remarks>
    /// Finds the asset by ID and verifies tenant ownership. Removes equipment models not present in the update, adds new models, and saves changes to the database. Throws an error if the asset is not found or belongs to another tenant.
    /// </remarks>
    Task UpdateAsset(UpdateAssetCommand command);

    /// <summary>
    /// Creates a new customer account for a tenant.
    /// </summary>
    /// <param name="command">The command containing customer account details.</param>
    /// <returns>The reply containing the new customer account's ID.</returns>
    /// <remarks>
    /// Checks for existing accounts with the same device serial and input code. If none exist, creates a new wallet using the payment system, adds a trustline for the asset, creates the account entity, and updates the device grain with input state. Saves all changes and returns the account ID.
    /// </remarks>
    Task<CreateCustomerAccountReply> CreateCustomerAccount(CreateCustomerAccountCommand command);

    /// <summary>
    /// Retrieves customer account details for a tenant.
    /// </summary>
    /// <param name="command">The command containing customer account and tenant IDs.</param>
    /// <returns>The reply with customer account details.</returns>
    /// <remarks>
    /// Finds the customer account by ID, verifies tenant ownership, and returns account details including wallet, asset, and state. Throws an error if not found or if the account belongs to another tenant.
    /// </remarks>
    Task<GetCustomerAccountReply> GetCustomerAccount(GetCustomerAccountCommand command);

    /// <summary>
    /// Creates a new invoice for a customer account.
    /// </summary>
    /// <param name="command">The command containing invoice details.</param>
    /// <returns>The reply containing the invoice XDR.</returns>
    /// <remarks>
    /// Validates the invoice amount and customer account state. Creates a new invoice entity, generates the XDR using the payment system, updates the invoice status, and commits the transaction. Rolls back the transaction if an error occurs.
    /// </remarks>
    Task<CreateInvoiceReply> CreateInvoice(CreateInvoiceCommand command);

    /// <summary>
    /// Lists invoices for a customer account within a specified period.
    /// </summary>
    /// <param name="command">The command containing filter parameters.</param>
    /// <returns>The reply containing a list of invoice items.</returns>
    /// <remarks>
    /// Retrieves invoices for the specified customer account and tenant within the given time period. Maps each invoice to a reply item including transaction ID, amount, XDR, and processed status.
    /// </remarks>
    Task<ListInvoicesReply> ListInvoices(ListInvoicesCommand command);

    /// <summary>
    /// Adds a new tenant to the system.
    /// </summary>
    /// <param name="command">The command containing tenant details.</param>
    /// <returns>The reply containing the new tenant's ID.</returns>
    /// <remarks>
    /// Creates a new <c>Tenant</c> entity and saves it to the database. Initializes the corresponding Orleans grain for the tenant and returns the tenant's ID.
    /// </remarks>
    Task<AddTenantReply> AddTenant(AddTenantCommand command);

    /// <summary>
    /// Updates an existing tenant's information.
    /// </summary>
    /// <param name="updateTenantCommand">The command containing updated tenant details.</param>
    /// <remarks>
    /// Finds the tenant by ID, updates its name and wallet, saves changes to the database, and updates the corresponding Orleans grain state.
    /// Throws an error if the tenant is not found.
    /// </remarks>
    Task UpdateTenant(UpdateTenantCommand updateTenantCommand);

    /// <summary>
    /// Deletes a customer account for a tenant.
    /// </summary>
    /// <param name="command">The command containing customer account and tenant IDs.</param>
    /// <remarks>
    /// Marks the account as deleting, then asynchronously attempts to remove the account from the payment system and database. If successful, marks the account as deleted and updates the device grain. If an error occurs, logs the error and restores the account state.
    /// </remarks>
    Task DeleteCustomerAccount(DeleteCustomerAccountCommand command);

    /// <summary>
    /// Lists customer accounts for a tenant with pagination.
    /// </summary>
    /// <param name="command">The command containing pagination and tenant information.</param>
    /// <returns>A paged list of customer account items.</returns>
    /// <remarks>
    /// Retrieves customer accounts for the specified tenant, applies pagination, and maps each account to a reply item with relevant details.
    /// </remarks>
    Task<Page<AccountItem>> ListCustomerAccounts(ListCustomerAccountsCommand command);

    /// <summary>
    /// Lists assets for a tenant with pagination.
    /// </summary>
    /// <param name="command">The command containing pagination and tenant information.</param>
    /// <returns>A paged list of asset items.</returns>
    /// <remarks>
    /// Retrieves assets for the specified tenant, applies pagination, and maps each asset to a reply item with relevant details.
    /// </remarks>
    Task<Page<AssetItem>> ListAssets(ListAssetsCommand command);
}
