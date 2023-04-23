using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;

namespace Utilities.Billing.Grains;
public class TenantGrain : Grain, ITenantGrain
{
    private readonly BillingDbContext _dbContext;

    public TenantGrain(BillingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<long> AddAccountTypeAsync(AddAccountTypeCommand command)
    {
        var entityEntry = await _dbContext.AccountTypes.AddAsync(new AccountType
        {
            TenantId = this.GetPrimaryKey(),
            Name = command.Name,
            Token = command.Token,
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
}
