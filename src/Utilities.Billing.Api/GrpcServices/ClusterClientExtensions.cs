using Grpc.Core;
using System.Security.Claims;
using Utilities.Billing.Contracts;

namespace Utilities.Billing.Api.GrpcServices;

public static class ClusterClientExtensions
{
    public static ITenantGrain GetTenant(this IGrainFactory clusterClient, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;

        var claimValue = user.FindFirstValue("client_tenant") ?? throw new InvalidOperationException("'client_tenant' claim is missing.");

        if (Guid.TryParse(claimValue, out var tenantId))
        {
            return clusterClient.GetGrain<ITenantGrain>(tenantId);
        }

        throw new InvalidOperationException("'client_tenant' claim is malformed.");
    }
}
