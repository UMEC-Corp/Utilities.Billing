using Utilities.Billing.Contracts;

namespace Utilities.Billing.Grains;
public class TenantGrain : Grain, ITenantGrain
{
    public Task<long> AddAccountTypeAsync(AddAccountTypeCommand command)
    {
        return Task.FromResult(0L);
    }
}
