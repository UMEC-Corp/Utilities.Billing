using Consul;
using Microsoft.AspNetCore.Authorization;

namespace Utilities.Billing.Api.Services;

[Authorize(Policy = "RequireScope")]
public class AccountsService : Protos.AccountsService.AccountsServiceBase
{
}
