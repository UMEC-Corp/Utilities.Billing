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

    public override async Task<GetAccountTypeResponse> GetAccountType(GetAccountTypeRequest request,
        ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(context);
        var accountType = await tenant.GetAccountTypeAsync(new GetAccountTypeQuery
        {
            Id = (long)request.Id,
        });

        return new GetAccountTypeResponse
        {
            AccountType = new AccountType
            {
                Id = (ulong)accountType.Id,
                Name = accountType.Name,
                Token = accountType.Token,
                Created = (ulong)accountType.Created.ToTimestamp(),
            }
        };
    }

    public override async Task<GetAccountTypesResponse> GetAccountTypes(GetAccountTypesRequest request,
        ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(context);
        var accountTypes = await tenant.GetAccountTypesAsync(new GetAccountTypesQuery
        {
            Offset = (int)request.Offset,
            Limit = (int)request.Limit,
        });

        return new GetAccountTypesResponse
        {
            AccountTypes =
            {
                accountTypes.Items.Select(x => new AccountType
                {
                    Id = (ulong)x.Id,
                    Name = x.Name,
                    Token = x.Token,
                    Created = (ulong)x.Created.ToTimestamp(),
                })
            }
        };
    }

    public override async Task<AddAccountTypeResponse> AddAccountType(AddAccountTypeRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(context);

        var id = await tenant.AddAccountTypeAsync(new AddAccountTypeCommand
        {
            Name = request.Name,
            Token = request.Token
        });

        return new AddAccountTypeResponse
        {
            Id = (ulong)id
        };
    }

    public override async Task<UpdateAccountTypeResponse> UpdateAccountType(UpdateAccountTypeRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(context);

        var command = new UpdateAccountTypeCommand();

        if (request.HasName)
        {
            command.Name = request.Name;
        }

        if (request.HasToken)
        {
            command.Token = request.Token;
        }

        await tenant.UpdateAccountTypeAsync(command);

        return new UpdateAccountTypeResponse();
    }

    public override async Task<DeleteAccountTypeResponse> DeleteAccountType(DeleteAccountTypeRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(context);

        var command = new DeleteAccountTypeCommand();

        await tenant.DeleteAccountTypeAsync(command);

        return new DeleteAccountTypeResponse();
    }
}
