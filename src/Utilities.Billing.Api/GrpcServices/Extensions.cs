using Grpc.Core;
using System.Security.Claims;
using Utilities.Billing.Contracts;

namespace Utilities.Billing.Api.GrpcServices;

public static class Extensions
{
    public static ITenantGrain GetTenant(this IGrainFactory clusterClient, string stringTenantId)
    {
        if (Guid.TryParse(stringTenantId, out var tenantId))
        {
            return clusterClient.GetGrain<ITenantGrain>(tenantId);
        }

        throw new InvalidOperationException("tenantId is invalid");
    }
}
