using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Utilities.Billing.Api.Extensions;
using Utilities.Billing.Api.Protos;
using Utilities.Billing.Contracts;

namespace Utilities.Billing.Api.GrpcServices;

[Authorize(Policy = "RequireBillingScope")]
public class BillingService : Protos.BillingService.BillingServiceBase
{
    private readonly IGrainFactory _clusterClient;

    public BillingService(IGrainFactory clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public override async Task<AddInvoiceResponse> AddInvoice(AddInvoiceRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(context);
        var reply = await tenant.AddInvoicesAsync(new AddInvoicesCommand
        {
            Items =
            {
                new AddInvoicesCommand.Item
                {
                    AccountId = (long)request.AccountId,
                    Amount = (decimal)request.Amount,
                    Date = request.HasDate ? request.Date.ToDateTime() : default(DateTime?),
                    DateTo =  request.HasDateTo ? request.DateTo.ToDateTime() : default(DateTime?),
                }
            }
        });

        return new AddInvoiceResponse
        {
            Id = (ulong)reply.InvoiceIds.Single()
        };
    }

    public override async Task<AddPaymentsForInvoicesResponse> AddPaymentsForInvoices(
        AddPaymentsForInvoicesRequest request, ServerCallContext context)
    {
        var tenant = _clusterClient.GetTenant(context);
        var reply = await tenant.AddPaymentsForInvoicesAsync(new AddPaymentsForInvoicesCommand
        {
            InvoiceIds = { request.InvoiceIds.Select(x => (long)x) }
        });

        return new AddPaymentsForInvoicesResponse
        {
            PaymentIds = { reply.PaymentIds.Select(x => (ulong)x) }
        };
    }
}