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
        var wallet = await _paymentSystem.CreateWalletAsync(new CreateWalletCommand(
            TenantId: this.GetPrimaryKey(),
            Token: command.Token
        ));

        var entityEntry = await _dbContext.AccountTypes.AddAsync(new AccountType
        {
            TenantId = this.GetPrimaryKey(),
            Name = command.Name,
            Token = command.Token,
            Wallet = wallet
        });

        await _dbContext.SaveChangesAsync();

        return entityEntry.Entity.Id;
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

    public async Task<AddPaymentsReply> AddPaymentsForInvoicesAsync(AddPaymentsForInvoicesCommand command)
    {
        CheckGrainState();

        var invoiceIds = command.InvoiceIds.Distinct().ToList();

        var invoices = await _dbContext.Invoices
            .Where(x => invoiceIds.Contains(x.Id))
            .ToListAsync();

        if (invoices.Count != invoiceIds.Count)
        {
            throw Errors.NotFound(nameof(Invoice), invoiceIds.Except(invoices.Select(x=>x.Id)).ToList());
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

    private void CheckGrainState()
    {
        if(_tenantState is null)
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
}

public static class Errors
{
    public static Exception NotFound(string entityName, ICollection<long> ids) =>
        throw new InvalidOperationException($"Entity not found {entityName}:{string.Join(",", ids)}");

    public static Exception GrainIsNotInitialized(string grainName, Guid id) =>
        throw new InvalidOperationException($"Grain uninitialized {grainName}:{id}");
}
