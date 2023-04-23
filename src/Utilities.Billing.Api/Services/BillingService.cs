using Consul;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Services;

[Authorize(Policy = "RequireScope")]
public class BillingService : Protos.BillingService.BillingServiceBase
{
}