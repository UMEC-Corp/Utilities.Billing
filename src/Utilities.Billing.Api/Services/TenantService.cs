using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Orleans;
using System.Globalization;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;
using Utilities.Billing.Grains;

namespace Utilities.Billing.Api.Services;

public class TenantService : ITenantService
{
    private readonly BillingDbContext _dbContext;
    private readonly IPaymentSystemFactory _paymentSystemFactory;
    private readonly IGrainFactory _clusterClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenantService> _logger;

    public TenantService(BillingDbContext dbContext,
                         IPaymentSystemFactory paymentSystemFactory,
                         IGrainFactory clusterClient,
                         IServiceProvider serviceProvider,
                         ILogger<TenantService> logger)
    {
        _dbContext = dbContext;
        _paymentSystemFactory = paymentSystemFactory;
        _clusterClient = clusterClient;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<AddTenantReply> AddTenant(AddTenantCommand command)
    {
        var tenant = new Tenant()
        {
            Name = command.Name,
            Wallet = command.Account,
            WalletType = command.WalletType,
        };

        await _dbContext.Tenants.AddAsync(tenant);
        await _dbContext.SaveChangesAsync();

        var tenantGrain = _clusterClient.GetGrain<ITenantGrain>(tenant.Id);
        await tenantGrain.GetState();

        return new AddTenantReply { Id = tenant.Id };
    }

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
        tenant.WalletType = command.WalletType;

        await _dbContext.SaveChangesAsync();

        await tenantGrain.UpdateState(new TenantGrainState { Name = tenant.Name, Wallet = tenant.Wallet, WalletType = tenant.WalletType });
    }

    public async Task<AddAssetReply> AddAsset(AddAssetCommand command)
    {
        var tenantId = GetTenantId(command.TenantId);
        var existsAsset = _dbContext.Assets.Where(x => x.Code == command.AssetCode && x.Issuer == command.Issuer && x.TenantId == tenantId);
        if (await existsAsset.AnyAsync())
        {
            throw Errors.EntityExists();
        }

        var ps = await GetPaymentSystemAsync(command.TenantId);
        await ps.AddAssetAsync(new AddStellarAssetCommand
        {
            AssetCode = command.AssetCode,
            IssuerAccountId = command.Issuer,
            ReceiverAccountId = await ps.GetMasterAccountAsync(),
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

    private async Task<IPaymentSystem> GetPaymentSystemAsync(string tenantId)
    {
        var tenantState = await _clusterClient.GetGrain<ITenantGrain>(Guid.Parse(tenantId)).GetState();
        return _paymentSystemFactory.GetPaymentSystem(tenantState.WalletType);
    }

    private Guid GetTenantId(string id)
    {
        return _clusterClient.GetGrain<ITenantGrain>(Guid.Parse(id)).GetPrimaryKey();
    }

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

        var ps = await GetPaymentSystemAsync(command.TenantId);
        var response = new GetAssetReply
        {
            Id = asset.Id,
            Code = asset.Code,
            IssuerAccount = asset.Issuer,
            MasterAccount = await ps.GetMasterAccountAsync(),
        };

        response.ModelCodes.Add(await _dbContext.EquipmentModels.Where(x => x.AssetId == asset.Id).Select(x => x.Code).ToListAsync());

        return response;
    }

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
        var ps = await GetPaymentSystemAsync(command.TenantId);
        var wallet = await ps.CreateWalletAsync(new CreateWalletCommand
        {
            CreateMuxed = command.CreateMuxed,
        });

        var asset = await GetAsset(new GetAssetCommand { Id = command.AssetId, TenantId = command.TenantId });

        await ps.AddAssetAsync(new AddStellarAssetCommand
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

        var ps = await GetPaymentSystemAsync(command.TenantId);
        return new GetCustomerAccountReply
        {
            Id = account.Id,
            Wallet = account.Wallet,
            AssetId = account.AssetId,
            AssetCode = account.Asset.Code,
            AssetIssuer = account.Asset.Issuer,
            MasterAccount = await ps.GetMasterAccountAsync(),
            State = account.State
        };
    }

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

                var ps = await GetPaymentSystemAsync(command.TenantId);
                await ps.DeleteCustomerAccountAsync(new DeleteCustomerAccount
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
                    deletingAccount.State = AccountState.Ok;  // ??
                    await dbContext.SaveChangesAsync();
                }
            }
        });

        return;
    }

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
            var ps = await GetPaymentSystemAsync(command.TenantId);
            var xdr = await ps.CreateInvoiceXdr(new CreateInvoiceXdrCommand
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
}
