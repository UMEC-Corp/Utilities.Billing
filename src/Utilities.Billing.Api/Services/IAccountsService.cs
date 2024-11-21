using Utilities.Billing.Contracts;

namespace Utilities.Billing.Api.Services;

public interface IAccountsService
{
    Task<MakePaymentReply> MakePaymentAsync(MakePaymentCommand command);
}