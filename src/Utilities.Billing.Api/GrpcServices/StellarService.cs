using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Orleans;
using System.Linq;
using Utilities.Billing.Api.Protos;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;
using Utilities.Billing.StellarWallets;

namespace Utilities.Billing.Api.GrpcServices;


[Authorize(Policy = "RequireBillingScope")]
public class StellarService : Protos.StellarService.StellarServiceBase
{
    private readonly IGrainFactory _clusterClient;

    public StellarService(IGrainFactory clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public override async Task<AddAssetResponse> AddAsset(AddAssetRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(context);
        var command = new AddAssetCommand
        {
            AssetCode = request.AssetCode,
            Issuer = request.Issuer,
        };
        command.ModelCodes.Add(request.ModelCodes);

        var reply = await tenant.AddAsset(command);

        return new AddAssetResponse { AssetId = reply.Id.ToString() };
    }

    public override async Task<GetAssetResponse> GetAsset(GetAssetRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(context);
        var command = new GetAssetCommand
        {
            Id = request.AssetId,
        };

        var reply = await tenant.GetAsset(command);

        var response = new GetAssetResponse
        {
            AssetId = reply.Id.ToString(),
            AssetCode = reply.Code,
            IssuerAccount = reply.IssuerAccount,
            MasterAccount = reply.MasterAccount,
        };
        response.ModelCodes.Add(response.ModelCodes);

        return response;
    }

    public override async Task<UpdateAssetResponse> UpdateAsset(UpdateAssetRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(context);
        var command = new UpdateAssetCommand
        {
            Id = request.AssetId,
        }; 
        command.ModelCodes.Add(request.ModelCodes);
        
        await tenant.UpdateAsset(command);        

        return new UpdateAssetResponse { };
    }
}

