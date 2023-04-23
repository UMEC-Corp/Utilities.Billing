using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Utilities.Billing.Api.Protos;
using Utilities.Billing.Contracts;

namespace Utilities.Billing.Api.GrpcServices;

[Authorize(Policy = "RequireScope")]
public class AccountsService : Protos.AccountsService.AccountsServiceBase
{
    private readonly IGrainFactory _clusterClient;

    public AccountsService(IGrainFactory clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public override async Task<AddAccountTypeResponse> AddAccountType(AddAccountTypeRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(context);

        var id = await tenant.AddAccountTypeAsync(new AddAccountTypeCommand
        {
            
        });

        return new AddAccountTypeResponse
        {
            Id = (ulong)id
        };
    }
}
