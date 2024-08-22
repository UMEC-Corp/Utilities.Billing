using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Orleans;
using System.Linq;
using Utilities.Billing.Api.Protos;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;
using Utilities.Billing.StellarWallets;

namespace Utilities.Billing.Api.GrpcServices;


[Authorize(Policy = "RequireBillingScope")]
public class StellarService : Protos.StellarService.StellarServiceBase
{
    private readonly IGrainFactory _clusterClient;
    private readonly BillingDbContext _dbContext;
    private readonly IOptionsMonitor<StellarWalletsSettings> _options;

    public StellarService(IGrainFactory clusterClient, BillingDbContext dbContext, IOptionsMonitor<StellarWalletsSettings> options)
    {
        _clusterClient = clusterClient;
        _dbContext = dbContext;
        _options = options;
    }

    public override async Task<AddAssetResponse> AddAsset(AddAssetRequest request, ServerCallContext context)
    {
        var asset = new Asset
        {
            Code = request.AssetCode,
        };
        await _dbContext.Assets.AddAsync(asset);

        foreach (var code in request.ModelCodes)
        {
            var model = new EquipmentModel
            {
                Code = code,
                Asset = asset
            };
            await _dbContext.EquipmentModels.AddAsync(model);
        }

        await _dbContext.SaveChangesAsync();

        return new AddAssetResponse { AssetId = asset.Id.ToString() };
    }

    public override async Task<GetAssetResponse> GetAsset(GetAssetRequest request, ServerCallContext context)
    {
        var asset = await _dbContext.Assets.FindAsync(request.AssetId);

        var response = new GetAssetResponse
        {
            AssetId = asset.Id.ToString(),
            AssetCode = asset.Code,
            IssuerAccount = asset.Issuer,
            MasterAccount = _options.CurrentValue.MassterAccount,
        };
        response.ModelCodes.Add(asset.EquipmentModels.Select(x => x.Code));

        return response;
    }

    public override async Task<UpdateAssetResponse> UpdateAsset(UpdateAssetRequest request, ServerCallContext context)
    {
        var asset = await _dbContext.Assets.FindAsync(request.AssetId);
        var existsModels = asset.EquipmentModels.Select(x => x.Code);

        var removingModels = asset.EquipmentModels.Where(x => !request.ModelCodes.Contains(x.Code)).ToList();
        foreach (var model in removingModels)
        {
            _dbContext.Remove(model);
        }

        var newModels = request.ModelCodes.Except(existsModels).ToList();
        foreach (var code in newModels)
        {
            await _dbContext.AddAsync(new EquipmentModel { Code = code, Asset = asset });
        }

        return new UpdateAssetResponse { };
    }
}
