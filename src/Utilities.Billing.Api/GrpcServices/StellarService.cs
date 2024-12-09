using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Orleans;
using StellarDotnetSdk;
using System.Linq;
using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Services;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data;
using Utilities.Billing.Data.Entities;
using Utilities.Billing.StellarWallets;

namespace Utilities.Billing.Api.GrpcServices;


[Authorize(Policy = "RequireScope")]
public class StellarService : Protos.StellarService.StellarServiceBase
{
    private readonly ITenantService _tenantService;

    public StellarService(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public override async Task<AddTenantResponse> AddTenant(AddTenantRequest request, ServerCallContext context)
    {
        var reply = await _tenantService.AddTenant(new AddTenantCommand
        {
            Name = request.Name,
            Account = request.Account,
            WalletType = Enum.Parse<WalletType>(request.WalletType)
        });

        return new AddTenantResponse { Id = reply.Id.ToString() };
    }

    public override async Task<UpdateTenantResponse> UpdateTenant(UpdateTenantRequest request, ServerCallContext context)
    {
        await _tenantService.UpdateTenant(new UpdateTenantCommand
        {
            TenantId = request.Id,
            Name = request.Name,
            Account = request.Account,
            WalletType = Enum.Parse<WalletType>(request.WalletType)
        });
        return new UpdateTenantResponse();
    }

    public override async Task<AddAssetResponse> AddAsset(AddAssetRequest request, ServerCallContext context)
    {
        var command = new AddAssetCommand
        {
            TenantId = request.TenantId,
            AssetCode = request.AssetCode,
            Issuer = request.Issuer,
        };
        command.ModelCodes.Add(request.ModelCodes);

        var reply = await _tenantService.AddAsset(command);

        return new AddAssetResponse { AssetId = reply.Id.ToString() };
    }

    public override async Task<GetAssetResponse> GetAsset(GetAssetRequest request, ServerCallContext context)
    {
        var command = new GetAssetCommand
        {
            TenantId = request.TenantId,
            Id = request.AssetId,
        };

        var reply = await _tenantService.GetAsset(command);

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
        var command = new UpdateAssetCommand
        {
            TenantId = request.TenantId,
            Id = request.AssetId,
        };
        command.ModelCodes.Add(request.ModelCodes);

        await _tenantService.UpdateAsset(command);

        return new UpdateAssetResponse { };
    }

    public override async Task<ListAssetsResponse> ListAssets(ListAssetsRequest request, ServerCallContext context)
    {
        var command = new ListAssetsCommand {
            TenantId = request.TenantId,
            Offset = request.HasOffset ? (int)request.Offset : default(int?),
            Limit = request.HasLimit ? (int)request.Limit : default(int?),
        };

        var reply = await _tenantService.ListAssets(command);
        var response = new ListAssetsResponse { Total = reply.Total };
        foreach (var item in reply.Items)
        {
            response.Items.Add(new ListAssetsResponse.Types.AssetsItem
            {
                AssetId = item.Id.ToString(),
                AssetCode = item.Code,
                IssuerAccount = item.Issuer,
            });
        }

        return response;
    }

    public override async Task<CreateCustomerAccountResponse> CreateCustomerAccount(CreateCustomerAccountRequest request, ServerCallContext context)
    {
        var command = new CreateCustomerAccountCommand
        {
            TenantId = request.TenantId,
            AssetId = request.AssetId,
            DeviceSerial = request.DeviceSerial,
            InputCode = request.InputCode,
            CreateMuxed = request.CreateMuxed,
        };

        var reply = await _tenantService.CreateCustomerAccount(command);

        return new CreateCustomerAccountResponse { CustomerAccountId = reply.AccountId.ToString() };
    }

    public override async Task<GetCustomerAccountResponse> GetCustomerAccount(GetCustomerAccountRequest request, ServerCallContext context)
    {
        var command = new GetCustomerAccountCommand
        {
            TenantId = request.TenantId,
            CustomerAccountId = request.CustomerAccountId,
        };

        var reply = await _tenantService.GetCustomerAccount(command);

        return new GetCustomerAccountResponse
        {
            CustomerAccountId = reply.Id.ToString(),
            CustomerAccount = reply.Wallet,
            AssetId = reply.AssetId.ToString(),
            Asset = reply.AssetCode,
            Issuer = reply.AssetIssuer,
            MasterAccount = reply.MasterAccount,
            State = reply.State.ToString(),
        };
    }

    public override async Task<ListCustomerAccountsResponse> ListCustomerAccounts(ListCustomerAccountsRequest request, ServerCallContext context)
    {
        var command = new ListCustomerAccountsCommand {
            TenantId = request.TenantId,
            Offset = request.HasOffset ? (int)request.Offset : default(int?),
            Limit = request.HasLimit ? (int)request.Limit : default(int?),
        };

        var reply = await _tenantService.ListCustomerAccounts(command);
        var response = new ListCustomerAccountsResponse { Total = reply.Total };
        foreach(var item in reply.Items)
        {
            response.Items.Add(new ListCustomerAccountsResponse.Types.AccountsItem
            {
                CustomerAccountId = item.Id.ToString(),
                CustomerAccount = item.Wallet,
                Asset = item.AssetCode,
                DeviceSerial = item.DeviceSerial,
                InputCode = item.InputCode,
                State = item.State.ToString(),
            });
        }

        return response;
    }

    public override async Task<DeleteCustomerAccountResponse> DeleteCustomerAccount(DeleteCustomerAccountRequest request, ServerCallContext context)
    {
        var command = new DeleteCustomerAccountCommand
        {
            TenantId = request.TenantId,
            CustomerAccountId = request.CustomerAccountId,
        };

        await _tenantService.DeleteCustomerAccount(command);

        return new DeleteCustomerAccountResponse { };
    }

    public override async Task<CreateInvoiceResponse> CreateInvoice(CreateInvoiceRequest request, ServerCallContext context)
    {
        var command = new CreateInvoiceCommand
        {
            TenantId = request.TenantId,
            CustomerAccountId = request.CustomerAccountId,
            PayerAccount = request.PayerAccount,
            Amount = request.Amount,
        };

        var reply = await _tenantService.CreateInvoice(command);

        return new CreateInvoiceResponse
        {
            InvoiceXdr = reply.Xdr,
        };
    }


    public override async Task<ListInvoicesResponse> ListInvoices(ListInvoicesRequest request, ServerCallContext context)
    {
        var command = new ListInvoicesCommand
        {
            TenantId = request.TenantId,
            CustomerAccountId = request.CustomerAccountId,
            PeriodFrom = request.PeriodFrom,
            PeriodTo = request.PeriodTo,
        };

        var reply = await _tenantService.ListInvoices(command);

        var response = new ListInvoicesResponse();
        foreach (var item in reply.Items)
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

