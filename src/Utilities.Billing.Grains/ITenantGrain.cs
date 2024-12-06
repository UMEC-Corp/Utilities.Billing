namespace Utilities.Billing.Grains;
public interface ITenantGrain : IGrainWithGuidKey
{
    Task UpdateState(TenantGrainState tenantGrainState);
    Task<TenantGrainState> GetState();
}
