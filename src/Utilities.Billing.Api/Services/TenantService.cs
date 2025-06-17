using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;
using Utilities.Billing.Grains;

namespace Utilities.Billing.Api.Services;

/// <summary>
/// Provides tenant-related operations, including tenant management, asset management, customer account management, and invoice processing.
/// </summary>
public class TenantService : ITenantService
{
    private readonly BillingDbContext _dbContext;
    private readonly IPaymentSystem _paymentSystem;
    private readonly IGrainFactory _clusterClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenantService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantService"/> class.
    /// </summary>
    /// <param name="dbContext">The billing database context.</param>
    /// <param name="paymentSystem">The payment system service.</param>
    /// <param name="clusterClient">The Orleans grain factory.</param>
    /// <param name="serviceProvider">The service provider for dependency resolution.</param>
    /// <param name="logger">The logger instance.</param>
    public TenantService(BillingDbContext dbContext, IPaymentSystem paymentSystem, IGrainFactory clusterClient, IServiceProvider serviceProvider, ILogger<TenantService> logger)
    {
        _dbContext = dbContext;
        _paymentSystem = paymentSystem;
        _clusterClient = clusterClient;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Adds a new tenant to the system.
    /// </summary>
    /// <param name="command">The command containing tenant details.</param>
    /// <returns>The reply containing the new tenant's ID.</returns>
    /// <remarks>
    /// Creates a new <c>Tenant</c> entity and saves it to the database. Initializes the corresponding Orleans grain for the tenant and returns the tenant's ID.
    /// </remarks>
    public async Task<AddTenantReply> AddTenant(AddTenantCommand command)
    {
        var tenant = new Tenant() { Name = command.Name, Wallet = command.Account };
        await _dbContext.Tenants.AddAsync(tenant);
        await _dbContext.SaveChangesAsync();

        var tenantGrain = _clusterClient.GetGrain<ITenantGrain>(tenant.Id);
        await tenantGrain.GetState();

        return new AddTenantReply { Id = tenant.Id };
    }

    /// <summary>
    /// Updates an existing tenant's information.
    /// </summary>
    /// <param name="command">The command containing updated tenant details.</param>
    /// <remarks>
    /// Finds the tenant by ID, updates its name and wallet, saves changes to the database, and updates the corresponding Orleans grain state.
    /// Throws an error if the tenant is not found.
    /// </remarks>
    public async Task UpdateTenant(UpdateTenantCommand command)
    {
        var tenantId = Guid.Parse(command.TenantId);
        var tenantGrain = _clusterClient.GetGrain<ITenantGrain>(tenantId);

        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant == null)
        {
            throw Errors.NotFound(nameof(Tenant), new List<string> { tenantId.ToString() });
        }

        tenant.Name = command.Name;
        tenant.Wallet = command.Account;

        await _dbContext.SaveChangesAsync();

        await tenantGrain.UpdateState(new TenantGrainState { Name = tenant.Name, Wallet = tenant.Wallet });
    }

    /// <summary>
    /// Adds a new asset for a tenant.
    /// </summary>
    /// <param name="command">The command containing asset details.</param>
    /// <returns>The reply containing the new asset's ID.</returns>
    /// <remarks>
    /// Checks if the asset already exists for the tenant. If not, adds a trustline for the asset using the payment system, creates the asset entity, and associates equipment models with it. Saves all changes to the database and returns the asset's ID.
    /// </remarks>
    public async Task<AddAssetReply> AddAsset(AddAssetCommand command)
    {
        var tenantId = GetTenantId(command.TenantId);
        var existsAsset = _dbContext.Assets.Where(x => x.Code == command.AssetCode && x.Issuer == command.Issuer && x.TenantId == tenantId);
        if (await existsAsset.AnyAsync())
        {
            throw Errors.EntityExists();
        }

        await _paymentSystem.AddAssetAsync(new AddStellarAssetCommand
        {
            AssetCode = command.AssetCode,
            IssuerAccountId = command.Issuer,
            ReceiverAccountId = await _paymentSystem.GetMasterAccountAsync(),
        });

        var asset = new Asset
        {
            Code = command.AssetCode,
            Issuer = command.Issuer,
            TenantId = tenantId,
        };
        await _dbContext.Assets.AddAsync(asset);

        await _dbContext.SaveChangesAsync();

        foreach (var code in command.ModelCodes)
        {
            var model = new EquipmentModel
            {
                Code = code,
                Asset = asset
            };
            await _dbContext.EquipmentModels.AddAsync(model);
        }

        await _dbContext.SaveChangesAsync();

        return new AddAssetReply { Id = asset.Id };
    }

    /// <summary>
    /// Retrieves asset details for a tenant.
    /// </summary>
    /// <param name="command">The command containing asset and tenant IDs.</param>
    /// <returns>The asset reply with asset details.</returns>
    /// <remarks>
    /// Finds the asset by ID and verifies tenant ownership. If found, returns asset details including associated model codes and master account information. Throws an error if not found or if the asset belongs to another tenant.
    /// </remarks>
    public async Task<GetAssetReply> GetAsset(GetAssetCommand command)
    {
        var asset = await _dbContext.Assets.FindAsync(new Guid(command.Id));
        if (asset == null)
        {
            throw Errors.NotFound(nameof(Asset), new List<string> { command.Id });
        }
        if (asset.TenantId != GetTenantId(command.TenantId))
        {
            throw Errors.BelongsAnotherTenant(nameof(Asset), command.Id);
        }

        var response = new GetAssetReply
        {
            Id = asset.Id,
            Code = asset.Code,
            IssuerAccount = asset.Issuer,
            MasterAccount = await _paymentSystem.GetMasterAccountAsync(),
        };

        response.ModelCodes.Add(await _dbContext.EquipmentModels.Where(x => x.AssetId == asset.Id).Select(x => x.Code).ToListAsync());

        return response;
    }

    /// <summary>
    /// Updates an asset and its associated equipment models.
    /// </summary>
    /// <param name="command">The command containing updated asset and model codes.</param>
    /// <remarks>
    /// Finds the asset by ID and verifies tenant ownership. Removes equipment models not present in the update, adds new models, and saves changes to the database. Throws an error if the asset is not found or belongs to another tenant.
    /// </remarks>
    public async Task UpdateAsset(UpdateAssetCommand command)
    {
        var asset = await _dbContext.Assets.FindAsync(new Guid(command.Id));
        if (asset == null)
        {
            throw Errors.NotFound(nameof(Asset), new List<string> { command.Id });
        }
        if (asset.TenantId != GetTenantId(command.TenantId))
        {
            throw Errors.BelongsAnotherTenant(nameof(Asset), command.Id);
        }

        var equipsQuery = _dbContext.EquipmentModels.Where(x => x.AssetId == asset.Id);
        var existsModels = await equipsQuery.Select(x => x.Code).ToListAsync();

        var removingModels = await equipsQuery.Where(x => !command.ModelCodes.Contains(x.Code)).ToListAsync();
        foreach (var model in removingModels)
        {
            _dbContext.EquipmentModels.Remove(model);
        }

        var newModels = command.ModelCodes.Except(existsModels).ToList();
        foreach (var code in newModels)
        {
            await _dbContext.EquipmentModels.AddAsync(new EquipmentModel { Code = code, Asset = asset });
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new customer account for a tenant.
    /// </summary>
    /// <param name="command">The command containing customer account details.</param>
    /// <returns>The reply containing the new customer account's ID.</returns>
    /// <remarks>
    /// Checks for existing accounts with the same device serial and input code. If none exist, creates a new wallet using the payment system, adds a trustline for the asset, creates the account entity, and updates the device grain with input state. Saves all changes and returns the account ID.
    /// </remarks>
    public async Task<CreateCustomerAccountReply> CreateCustomerAccount(CreateCustomerAccountCommand command)
    {
        var tenantId = GetTenantId(command.TenantId);
        var existsAccount = _dbContext.Accounts
            .Where(x => x.TenantId == tenantId)
            .Where(x => x.DeviceSerial == command.DeviceSerial && x.InputCode == command.InputCode)
            .Where(x => x.State != AccountState.Deleted);
        if (await existsAccount.AnyAsync())
        {
            throw Errors.EntityExists();
        }

        var wallet = await _paymentSystem.CreateWalletAsync(new CreateWalletCommand
        {
            CreateMuxed = command.CreateMuxed,
        });

        var asset = await GetAsset(new GetAssetCommand { Id = command.AssetId, TenantId = command.TenantId });

        await _paymentSystem.AddAssetAsync(new AddStellarAssetCommand
        {
            AssetCode = asset.Code,
            IssuerAccountId = asset.IssuerAccount,
            ReceiverAccountId = wallet
        });

        var account = new Account
        {
            TenantId = GetTenantId(command.TenantId),
            DeviceSerial = command.DeviceSerial,
            InputCode = command.InputCode,
            AssetId = asset.Id,
            Wallet = wallet,
            State = AccountState.Ok
        };

        await _dbContext.Accounts.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        var deviceGrain = _clusterClient.GetGrain<IDeviceGrain>(account.DeviceSerial);
        await deviceGrain.UpdateInputState(account.InputCode, new InputInfo
        {
            AccountId = account.Id,
            DeviceSerial = account.DeviceSerial,
            Wallet = account.Wallet,
            AssetCode = asset.Code,
            AssetId = asset.Id,
            AssetIssuer = asset.IssuerAccount,
        });

        return new CreateCustomerAccountReply { AccountId = account.Id };
    }

    /// <summary>
    /// Retrieves customer account details for a tenant.
    /// </summary>
    /// <param name="command">The command containing customer account and tenant IDs.</param>
    /// <returns>The reply with customer account details.</returns>
    /// <remarks>
    /// Finds the customer account by ID, verifies tenant ownership, and returns account details including wallet, asset, and state. Throws an error if not found or if the account belongs to another tenant.
    /// </remarks>
    public async Task<GetCustomerAccountReply> GetCustomerAccount(GetCustomerAccountCommand command)
    {
        var tenantId = GetTenantId(command.TenantId);
        var account = await _dbContext.Accounts
            .Include(x => x.Asset)
            .FirstOrDefaultAsync(x => x.Id == new Guid(command.CustomerAccountId));
        if (account == null)
        {
            throw Errors.NotFound(nameof(Account), new List<string> { command.CustomerAccountId });
        }
        if (account.TenantId != tenantId)
        {
            throw Errors.BelongsAnotherTenant(nameof(Asset), command.CustomerAccountId);
        }

        return new GetCustomerAccountReply
        {
            Id = account.Id,
            Wallet = account.Wallet,
            AssetId = account.AssetId,
            AssetCode = account.Asset.Code,
            AssetIssuer = account.Asset.Issuer,
            MasterAccount = await _paymentSystem.GetMasterAccountAsync(),
            State = account.State
        };
    }

    /// <summary>
    /// Deletes a customer account for a tenant.
    /// </summary>
    /// <param name="command">The command containing customer account and tenant IDs.</param>
    /// <remarks>
    /// Marks the account as deleting, then asynchronously attempts to remove the account from the payment system and database. If successful, marks the account as deleted and updates the device grain. If an error occurs, logs the error and restores the account state.
    /// </remarks>
    public async Task DeleteCustomerAccount(DeleteCustomerAccountCommand command)
    {
        var tenantId = GetTenantId(command.TenantId);
        var accountId = new Guid(command.CustomerAccountId);
        var account = await _dbContext.Accounts
             .Include(x => x.Asset)
             .FirstOrDefaultAsync(x => x.Id == accountId && x.TenantId == tenantId);
        if (account == null)
        {
            throw Errors.NotFound(nameof(Account), new List<string> { command.CustomerAccountId });
        }

        if (account.State != AccountState.Ok)
        {
            throw Errors.IncorrectState(nameof(Account), account.State.ToString());
        }
        account.State = AccountState.Deleting;
        await _dbContext.SaveChangesAsync();

        _ = Task.Run(async () =>
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
            Account? deletingAccount = null;
            try
            {
                deletingAccount = await dbContext.Accounts.Include(x => x.Asset).FirstOrDefaultAsync(x => x.Id == accountId && x.TenantId == tenantId);
                if (deletingAccount == null)
                {
                    throw Errors.NotFound(nameof(Account), new List<string> { command.CustomerAccountId });
                }

                await _paymentSystem.DeleteCustomerAccountAsync(new DeleteCustomerAccount
                {
                    CustomerAccountId = deletingAccount.Wallet,
                    AssetCode = deletingAccount.Asset.Code,
                    AssetIssuerAccountId = deletingAccount.Asset.Issuer
                });

                deletingAccount.State = AccountState.Deleted;
                await dbContext.SaveChangesAsync();

                var deviceGrain = _clusterClient.GetGrain<IDeviceGrain>(deletingAccount.DeviceSerial);
                await deviceGrain.DeleteInputState(deletingAccount.InputCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete account {AccountId}", accountId);

                if (deletingAccount != null)
                {
                    deletingAccount.State = AccountState.Ok;
                    await dbContext.SaveChangesAsync();
                }
            }
        });

        return;
    }

    /// <summary>
    /// Creates a new invoice for a customer account.
    /// </summary>
    /// <param name="command">The command containing invoice details.</param>
    /// <returns>The reply containing the invoice XDR.</returns>
    /// <remarks>
    /// Validates the invoice amount and customer account state. Creates a new invoice entity, generates the XDR using the payment system, updates the invoice status, and commits the transaction. Rolls back the transaction if an error occurs.
    /// </remarks>
    public async Task<CreateInvoiceReply> CreateInvoice(CreateInvoiceCommand command)
    {
        var tenantState = await _clusterClient.GetGrain<ITenantGrain>(Guid.Parse(command.TenantId)).GetState();
        if (!decimal.TryParse(command.Amount, CultureInfo.InvariantCulture, out var amount) || amount == 0M)
        {
            throw Errors.InvalidValue(nameof(command.Amount), command.Amount);
        }
        await _dbContext.Database.BeginTransactionAsync();

        var customerAccount = await GetCustomerAccount(new GetCustomerAccountCommand { CustomerAccountId = command.CustomerAccountId, TenantId = command.TenantId });
        if (customerAccount.State != AccountState.Ok)
        {
            throw Errors.IncorrectState(nameof(Account), customerAccount.State.ToString());
        }

        var invoice = new Invoice
        {
            AccountId = customerAccount.Id,
            PayerWallet = command.PayerAccount,
            Amount = amount,
            Status = InvoiceStatus.New,
        };
        await _dbContext.Invoices.AddAsync(invoice);
        await _dbContext.SaveChangesAsync();

        try
        {
            var xdr = await _paymentSystem.CreateInvoiceXdr(new CreateInvoiceXdrCommand
            {
                InvoiceId = invoice.Id,
                Amount = amount,
                AssetCode = customerAccount.AssetCode,
                AssetsIssuerAccountId = customerAccount.AssetIssuer,
                TenantAccountId = tenantState.Wallet,
                DeviceAccountId = customerAccount.Wallet,
                PayerAccountId = command.PayerAccount
            });

            invoice.Xdr = xdr;
            invoice.Status = InvoiceStatus.Pending;
            await _dbContext.SaveChangesAsync();

            await _dbContext.Database.CommitTransactionAsync();

            return new CreateInvoiceReply
            {
                Xdr = xdr,
            };
        }
        catch
        {
            await _dbContext.Database.RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Lists invoices for a customer account within a specified period.
    /// </summary>
    /// <param name="command">The command containing filter parameters.</param>
    /// <returns>The reply containing a list of invoice items.</returns>
    /// <remarks>
    /// Retrieves invoices for the specified customer account and tenant within the given time period. Maps each invoice to a reply item including transaction ID, amount, XDR, and processed status.
    /// </remarks>
    public async Task<ListInvoicesReply> ListInvoices(ListInvoicesCommand command)
    {
        var start = DateTimeOffset.FromUnixTimeSeconds((long)command.PeriodFrom);
        var stop = DateTimeOffset.FromUnixTimeSeconds((long)command.PeriodTo);
        var tenantId = GetTenantId(command.TenantId);

        var invoices = await _dbContext.Invoices
            .Where(x => x.AccountId == new Guid(command.CustomerAccountId) && x.Account.TenantId == tenantId)
            .Where(x => x.Created >= start && x.Created <= stop)
            .Select(x => new
            {
                x.Id,
                x.Amount,
                x.Xdr,
                x.Status
            })
            .ToListAsync();

        var reply = new ListInvoicesReply();
        foreach (var invoice in invoices)
        {
            reply.Items.Add(new ListInvoicesReply.InvoiceItem
            {
                TransactionId = invoice.Id.ToString(),
                Amount = invoice.Amount.ToString(CultureInfo.InvariantCulture),
                Xdr = invoice.Xdr,
                Processed = invoice.Status == InvoiceStatus.Completed ? true : false,
            });
        }

        return reply;
    }

    /// <summary>
    /// Lists customer accounts for a tenant with pagination.
    /// </summary>
    /// <param name="command">The command containing pagination and tenant information.</param>
    /// <returns>A paged list of customer account items.</returns>
    /// <remarks>
    /// Retrieves customer accounts for the specified tenant, applies pagination, and maps each account to a reply item with relevant details.
    /// </remarks>
    public async Task<Page<AccountItem>> ListCustomerAccounts(ListCustomerAccountsCommand command)
    {
        var tenantId = GetTenantId(command.TenantId);
        var query = _dbContext.Accounts.Where(x => x.TenantId == tenantId);
        var total = await query.CountAsync();

        query = query.OrderBy(x => x.Id).Skip(command.Offset ?? 0).Take(command.Limit ?? 10);

        var accounts = await query
            .Select(x => new
            {
                x.Id,
                x.Wallet,
                AssetCode = x.Asset.Code,
                x.DeviceSerial,
                x.InputCode,
                x.State
            })
            .ToListAsync();

        var reply = new Page<AccountItem>() { Total = total };
        foreach (var account in accounts)
        {
            reply.Items.Add(new AccountItem
            {
                Id = account.Id,
                Wallet = account.Wallet,
                AssetCode = account.AssetCode,
                DeviceSerial = account.DeviceSerial,
                InputCode = account.InputCode,
                State = account.State
            });
        }

        return reply;
    }

    /// <summary>
    /// Lists assets for a tenant with pagination.
    /// </summary>
    /// <param name="command">The command containing pagination and tenant information.</param>
    /// <returns>A paged list of asset items.</returns>
    /// <remarks>
    /// Retrieves assets for the specified tenant, applies pagination, and maps each asset to a reply item with relevant details.
    /// </remarks>
    public async Task<Page<AssetItem>> ListAssets(ListAssetsCommand command)
    {
        var tenantId = GetTenantId(command.TenantId);
        var query = _dbContext.Assets.Where(x => x.TenantId == tenantId);
        var total = await query.CountAsync();

        query = query.OrderBy(x => x.Id).Skip(command.Offset ?? 0).Take(command.Limit ?? 10);

        var assets = await query
            .Where(x => x.TenantId == tenantId)
            .Select(x => new
            {
                x.Id,
                x.Code,
                x.Issuer
            })
            .ToListAsync();

        var reply = new Page<AssetItem>() { Total = total };
        foreach (var asset in assets)
        {
            reply.Items.Add(new AssetItem
            {
                Id = asset.Id,
                Code = asset.Code,
                Issuer = asset.Issuer
            });
        }

        return reply;
    }

    /// <summary>
    /// Gets the tenant ID as a <see cref="Guid"/> from a string identifier.
    /// </summary>
    /// <param name="id">The tenant ID as a string.</param>
    /// <returns>The tenant ID as a <see cref="Guid"/>.</returns>
    /// <remarks>
    /// Uses the Orleans grain factory to resolve the primary key for the tenant grain.
    /// </remarks>
    private Guid GetTenantId(string id)
    {
        return _clusterClient.GetGrain<ITenantGrain>(Guid.Parse(id)).GetPrimaryKey();
    }
}
