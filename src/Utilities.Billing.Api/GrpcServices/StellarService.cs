using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Orleans;
using StellarDotnetSdk;
using System.Linq;
using Utilities.Billing.Api.Protos;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;
using Utilities.Billing.StellarWallets;

namespace Utilities.Billing.Api.GrpcServices;


[Authorize(Policy = "RequireScope")]
public class StellarService : Protos.StellarService.StellarServiceBase
{
    private readonly IGrainFactory _clusterClient;

    public StellarService(IGrainFactory clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public override async Task<AddAssetResponse> AddAsset(AddAssetRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetGrain<ITenantGrain>(Guid.Parse(request.TenantId));
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
        var tenant = _clusterClient.GetGrain<ITenantGrain>(Guid.Parse(request.TenantId));
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
        response.ModelCodes.Add(reply.ModelCodes);

        return response;
    }

    public override async Task<UpdateAssetResponse> UpdateAsset(UpdateAssetRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetGrain<ITenantGrain>(Guid.Parse(request.TenantId));
        var command = new UpdateAssetCommand
        {
            Id = request.AssetId,
        };
        command.ModelCodes.Add(request.ModelCodes);

        await tenant.UpdateAsset(command);

        return new UpdateAssetResponse { };
    }

    public override async Task<CreateCustomerAccountResponse> CreateCustomerAccount(CreateCustomerAccountRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(request.TenantId);
        var command = new CreateCustomerAccountCommand
        {
            AssetId = request.AssetId,
            ControllerSerial = request.ControllerSerial,
            MeterNumber = request.MeterNumber,
            CreateMuxed = request.CreateMuxed,
        };

        var reply = await tenant.CreateCustomerAccount(command);

        return new CreateCustomerAccountResponse { CustomerAccountId = reply.AccountId.ToString() };
    }

    public override async Task<GetCustomerAccountResponse> GetCustomerAccount(GetCustomerAccountRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(request.TenantId);
        var command = new GetCustomerAccountCommand
        {
            CustomerAccountId = request.CustomerAccountId,
        };

        var reply = await tenant.GetCustomerAccount(command);

        return new GetCustomerAccountResponse
        {
            CustomerAccountId = reply.Id.ToString(),
            CustomerAccount = reply.Wallet,
            AssetId = reply.AssetId.ToString(),
            Asset = reply.AssetCode,
            Issuer = reply.AssetIssuer,
            MasterAccount = reply.MasterAccount,
        };
    }

    public override async Task<CreateInvoiceResponse> CreateInvoice(CreateInvoiceRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(request.TenantId);

        var command = new CreateInvoiceCommand
        {
            CustomerAccountId = request.CustomerAccountId,
            PayerAccount = request.PayerAccount,
            Amount = request.Amount,
        };

        var reply = await tenant.CreateInvoice(command);

        return new CreateInvoiceResponse
        {
            InvoiceXdr = reply.Xdr,
        };
    }


    public override async Task<ListInvoicesResponse> ListInvoices(ListInvoicesRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(request.TenantId);

        var command = new ListInvoicesCommand
        {
            CustomerAccountId = request.CustomerAccountId,
            PeriodFrom = request.PeriodFrom,
            PeriodTo = request.PeriodTo,
        };

        var reply = await tenant.ListInvoices(command);

        var response = new ListInvoicesResponse();
        foreach ( var item in reply.Items )
        {
            response.Items.Add(new ListInvoicesResponse.Types.InvoicesListItem
            {
                TransactionId = item.TransactionId,
                Amount = item.Amount,
                Xdr = item.Xdr,
                Processed = item.Processed,
            });
        }

        return response;
    }
}

