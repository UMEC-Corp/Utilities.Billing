using Microsoft.EntityFrameworkCore;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;

namespace Utilities.Billing.Grains;
public class TenantGrain : Grain, ITenantGrain
{
    private readonly BillingDbContext _dbContext;
    private readonly IPaymentSystem _paymentSystem;
    private Tenant _tenantState;

    public TenantGrain(BillingDbContext dbContext, IPaymentSystem paymentSystem)
    {
        _dbContext = dbContext;
        _paymentSystem = paymentSystem;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _tenantState = await _dbContext.Tenants.FindAsync(this.GetPrimaryKey());
    }

    public async Task<long> AddAccountTypeAsync(AddAccountTypeCommand command)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAccountTypeAsync(DeleteAccountTypeCommand command)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAccountTypeAsync(UpdateAccountTypeCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<Page<AccountTypeItem>> GetAccountTypesAsync(GetAccountTypesQuery query)
    {
        throw new NotImplementedException();
    }

    public Task<AccountTypeItem> GetAccountTypeAsync(GetAccountTypeQuery query)
    {
        throw new NotImplementedException();
    }

    /*
    public async Task<AddPaymentsReply> AddPaymentsForInvoicesAsync(AddPaymentsForInvoicesCommand command)
    {
        CheckGrainState();

        var invoiceIds = command.InvoiceIds.Distinct().ToList();

        var invoices = await _dbContext.Invoices
            .Where(x => invoiceIds.Contains(x.Id))
            .ToListAsync();

        if (invoices.Count != invoiceIds.Count)
        {
            throw Errors.NotFound(nameof(Invoice), invoiceIds.Except(invoices.Select(x => x.Id)).ToList());
        }

        // Preload accounts to the db context
        var accountIds = invoices.Select(x => x.AccountId).Distinct().ToList();

        var accounts = await _dbContext.Accounts
            .Include(x => x.AccountType)
            .Include(x => x.AccountHolder)
            .Where(x => accountIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        var rates = await GetRatesAsync(invoices
            .Select(x => x.Account.AccountTypeId)
            .Distinct());

        var absentRates = rates
            .Where(x => x.Value is not { SellPrice: > 0 })
            .Select(x => x.Key)
            .ToList();

        if (absentRates.Any())
        {
            throw Errors.NotFound("Exchange Rate for Account Type", absentRates.ToList());
        }

        var reply = new AddPaymentsReply();

        var groups = invoices
            .Where(x => x.AmountTotal - x.AmountPayed > 0)
            .GroupBy(x => x.Account.Id);

        foreach (var group in groups)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var account = accounts[group.Key];

                var tokenAmount = group.Sum(x => x.AmountTotal - x.AmountPayed);

                var rate = rates[account.AccountTypeId];

                var currencyAmount = tokenAmount * rate!.SellPrice;

                var payment = new Payment
                {
                    AccountId = account.Id,
                    TokenAmount = tokenAmount,
                    CurrencyAmount = currencyAmount,
                    Date = DateTime.UtcNow,
                    DateTo = rate.Expires,
                    Status = PaymentStatus.Initial,
                };

                await _dbContext.Payments.AddAsync(payment);

                await _dbContext.SaveChangesAsync();

                await _paymentSystem.AddPaymentAsync(new AddPaymentCommand(
                    TenantId: this.GetPrimaryKey(),
                    PaymentId: payment.Id,
                    BuyToken: account.AccountType.Token,
                    BuyTokenAmount: tokenAmount,
                    SellCurrency: _tenantState.Currency,
                    SellCurrencyAmount: currencyAmount,
                    BuyerCurrencyWallet: account.AccountHolder.Wallet,
                    SellerCurrencyWallet: _tenantState.Wallet,
                    BuyerTokenWallet: account.Wallet,
                    SellerTokenWallet: account.AccountType.Wallet
                ));

                reply.PaymentIds.Add(payment.Id);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        return reply;
    }
    */

    private void CheckGrainState()
    {
        if (_tenantState is null)
        {
            throw Errors.GrainIsNotInitialized(nameof(TenantGrain), this.GetPrimaryKey());
        }
    }

    private async Task<Dictionary<long, ExchangeRate?>> GetRatesAsync(IEnumerable<long> accountTypes)
    {
        var rates = await _dbContext.ExchangeRates
            .Where(x => accountTypes.Contains(x.AccountTypeId))
            .Where(x => x.Effective <= DateTime.UtcNow)
            .ToListAsync();

        var groups = rates
            .GroupBy(x => x.AccountTypeId);

        var result = accountTypes.ToDictionary(x => x, _ => (ExchangeRate?)null);

        foreach (var group in groups)
        {
            var rate = group
                .MaxBy(x => x.Expires ?? DateTime.MaxValue);

            result[group.Key] = rate;
        }

        return result;
    }

    /*
    public async Task<AddInvoicesReply> AddInvoicesAsync(AddInvoicesCommand command)
    {
        var accountIds = command.Items.Select(x => x.AccountId).Distinct().ToList();

        var accounts = await _dbContext.Accounts
            .Where(x => accountIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        if (accounts.Count != accountIds.Count)
        {
            throw Errors.NotFound(nameof(Account), accountIds.Except(accounts.Keys).ToList());
        }

        var reply = new AddInvoicesReply();

        foreach (var item in command.Items)
        {
            var entityEntry = await _dbContext.Invoices.AddAsync(new Invoice
            {
                Account = accounts[item.AccountId],
                AmountTotal = item.Amount,
                AmountPayed = 0,
                Date = item.Date ?? DateTime.Now,
                DateTo = item.DateTo,
            });

            reply.InvoiceIds.Add(entityEntry.Entity.Id);
        }

        await _dbContext.SaveChangesAsync();

        return reply;
    }
    */
    public Task<AddPaymentsReply> AddPaymentsForInvoicesAsync(AddPaymentsForInvoicesCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<AddInvoicesReply> AddInvoicesAsync(AddInvoicesCommand addInvoicesCommand)
    {
        throw new NotImplementedException();
    }


    public async Task<AddAssetReply> AddAsset(AddAssetCommand command)
    {
        var tenantId = this.GetPrimaryKey();
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

    public async Task<GetAssetReply> GetAsset(GetAssetCommand command)
    {
        var asset = await _dbContext.Assets.FindAsync(new Guid(command.Id));
        if (asset == null)
        {
            throw Errors.NotFound(nameof(Asset), new List<string> { command.Id });
        }
        if (asset.TenantId != this.GetPrimaryKey())
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
        //response.ModelCodes.Add(asset.EquipmentModels.Select(x => x.Code));
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
        if (asset.TenantId != this.GetPrimaryKey())
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
        var wallet = await _paymentSystem.CreateWalletAsync(new CreateWalletCommand
        {
            CreateMuxed = command.CreateMuxed,
        });

        var asset = await GetAsset(new GetAssetCommand { Id = command.AssetId });

        await _paymentSystem.AddAssetAsync(new AddStellarAssetCommand
        {
            AssetCode = asset.Code,
            IssuerAccountId = asset.IssuerAccount,
            ReceiverAccountId = wallet
        });

        var entityEntry = await _dbContext.Accounts.AddAsync(new Account
        {
            TenantId = this.GetPrimaryKey(),
            ControllerSerial = command.ControllerSerial,
            MeterNumber = command.MeterNumber,
            AssetId = asset.Id,
            Wallet = wallet
        });

        await _dbContext.SaveChangesAsync();

        return new CreateCustomerAccountReply { AccountId = entityEntry.Entity.Id };
    }

    public async Task<GetCustomerAccountReply> GetCustomerAccount(GetCustomerAccountCommand command)
    {
        var tenantId = this.GetPrimaryKey();
        var account = await _dbContext.Accounts
            .Include(x => x.Asset)
            .FirstOrDefaultAsync(x => x.Id == new Guid(command.CustomerAccountId));
        if (account == null)
        {
            throw Errors.NotFound(nameof(Account), new List<string> { command.CustomerAccountId });
        }

        return new GetCustomerAccountReply
        {
            Id = account.Id,
            Wallet = account.Wallet,
            AssetId = account.AssetId,
            AssetCode = account.Asset.Code,
            AssetIssuer = account.Asset.Issuer,
            MasterAccount = await _paymentSystem.GetMasterAccountAsync()
        };
    }

    public async Task<CreateInvoiceReply> CreateInvoice(CreateInvoiceCommand command)
    {
        if (!decimal.TryParse(command.Amount, out var amount) || amount == 0M)
        {
            throw Errors.InvalidValue(nameof(command.Amount), command.Amount);
        }

        var customerAccount = await GetCustomerAccount(new GetCustomerAccountCommand { CustomerAccountId = command.CustomerAccountId });

        var xdr = await _paymentSystem.CreateInvoiceXdr(new CreateInvoiceXdrCommand
        {
            Amount = command.Amount,
            AssetCode = customerAccount.AssetCode,
            AssetsIssuerAccountId = customerAccount.AssetIssuer,
            DeviceAccountId = customerAccount.Wallet,
            PayerAccountId = command.PayerAccount
        });

        var invoice = new Invoice
        {
            AccountId = customerAccount.Id,
            PayerWallet = command.PayerAccount,
            Amount = amount,
            Xdr = xdr
        };
        await _dbContext.Invoices.AddAsync(invoice);
        await _dbContext.SaveChangesAsync();

        return new CreateInvoiceReply
        {
            Xdr = xdr,
        };
    }

    public static class Errors
    {
        public static Exception NotFound(string entityName, ICollection<long> ids) =>
            throw new InvalidOperationException($"Entity not found {entityName}:{string.Join(",", ids)}");

        public static Exception NotFound(string entityName, ICollection<string> ids) =>
            throw new InvalidOperationException($"Entity not found {entityName}:{string.Join(",", ids)}");

        public static Exception GrainIsNotInitialized(string grainName, Guid id) =>
            throw new InvalidOperationException($"Grain uninitialized {grainName}:{id}");

        public static Exception EntityExists()
        {
            throw new InvalidOperationException($"Entity already exists");
        }

        public static Exception BelongsAnotherTenant(string entityName, string id)
        {
            throw new InvalidOperationException($"Entity {entityName}:{id} belongs another Tenant");
        }

        internal static Exception InvalidValue(string field, string value)
        {
            throw new InvalidOperationException($"{field} has ivalid value: {value}");
        }
    }
}
