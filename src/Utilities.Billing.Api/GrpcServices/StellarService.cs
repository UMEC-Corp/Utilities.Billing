using Microsoft.AspNetCore.Authorization;

namespace Utilities.Billing.Api.GrpcServices;


[Authorize(Policy = "RequireBillingScope")]
public class StellarService : Protos.StellarService.StellarServiceBase
{
}
