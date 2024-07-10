using Microsoft.AspNetCore.Authorization;

namespace Utilities.Billing.Api.GrpcServices;


/// <summary>
/// API для выставления инвойсов за потребленные услуги ЖКХ в Stellar.
/// </summary>
[Authorize(Policy = "RequireBillingScope")]
public class StellarService : Protos.StellarService.StellarServiceBase
{
}
