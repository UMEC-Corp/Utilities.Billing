using Microsoft.AspNetCore.Authorization;

namespace Utilities.Billing.Api.GrpcServices;

[Authorize(Policy = "RequireScope")]
public class BillingService : Protos.BillingService.BillingServiceBase
{
}