using Utilities.Billing.Contracts;
using Utilities.Billing.Data.Entities;

namespace Utilities.Billing.Api.Services;

/// <summary>
/// Provides account-related operations, including making payments and updating device/account state.
/// </summary>
public interface IAccountsService
{
    /// <summary>
    /// Makes a payment for a device input and updates the account and device state accordingly.
    /// </summary>
    /// <param name="command">The command containing payment details, including device serial, input code, and incoming value.</param>
    /// <returns>A <see cref="MakePaymentReply"/> indicating the result of the operation.</returns>
    Task<MakePaymentReply> MakePaymentAsync(MakePaymentCommand command);
}
