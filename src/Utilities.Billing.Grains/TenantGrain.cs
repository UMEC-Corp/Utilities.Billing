using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;

namespace Utilities.Billing.Grains;
public partial class TenantGrain : Grain, ITenantGrain
{
    private readonly BillingDbContext _dbContext;
    private readonly IPaymentSystem _paymentSystem;
    private TenantGrainState _tenantState;

    public TenantGrain(BillingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _tenantState = _dbContext.Tenants
            .Where(x => x.Id == this.GetPrimaryKey())
            .Select(x => new TenantGrainState { Name = x.Name, Wallet = x.Wallet })
            .FirstOrDefault();

        return Task.CompletedTask;
    }

    public Task<TenantGrainState> GetState()
    {
        return Task.FromResult(_tenantState);
    }

    public  Task UpdateState(TenantGrainState tenantGrainState)
    {
        _tenantState = tenantGrainState;

        return Task.CompletedTask;
    }
}
