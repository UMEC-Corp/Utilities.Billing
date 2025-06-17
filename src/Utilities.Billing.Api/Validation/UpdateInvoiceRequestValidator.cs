using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

/// <summary>
/// Validator for <see cref="UpdateInvoiceRequest"/>.
/// </summary>
/// <remarks>
/// This validator enforces the following rules for <see cref="UpdateInvoiceRequest"/>:
/// <list type="bullet">
///   <item>
///     <description><c>Id</c> must not be empty.</description>
///   </item>
///   <item>
///     <description><c>Amount</c> must be greater than 0 if <c>HasAmount</c> is true.</description>
///   </item>
///   <item>
///     <description><c>AccountId</c> must not be empty if <c>HasAccountId</c> is true.</description>
///   </item>
///   <item>
///     <description><c>Date</c> must be greater than or equal to the current UTC time (in Unix seconds) if <c>HasDate</c> is true.</description>
///   </item>
///   <item>
///     <description><c>DateTo</c> must be greater than or equal to the current UTC time (in Unix seconds) if <c>HasDateTo</c> is true.</description>
///   </item>
///   <item>
///     <description><c>DateTo</c> must be greater than <c>Date</c> if <c>HasDateTo</c> is true.</description>
///   </item>
/// </list>
/// These rules ensure that all required fields are present and that date and amount values are logically valid for invoice updates.
/// </remarks>
public class UpdateInvoiceRequestValidator : AbstractValidator<UpdateInvoiceRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateInvoiceRequestValidator"/> class.
    /// </summary>
    /// <remarks>
    /// Sets up validation rules for invoice update requests, including checks for required fields, positive amounts, and valid date ranges.
    /// </remarks>
    public UpdateInvoiceRequestValidator()
    {
        var now = (ulong)DateTime.UtcNow.ToUnixTimeSeconds();

        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(.0).When(x => x.HasAmount);
        RuleFor(x => x.AccountId).NotEmpty().When(x => x.HasAccountId);
        RuleFor(x => x.Date).GreaterThanOrEqualTo(now).When(request => request.HasDate);
        RuleFor(x => x.DateTo).GreaterThanOrEqualTo(now).When(request => request.HasDateTo);
        RuleFor(x => x.DateTo).GreaterThan(x => x.Date).When(request => request.HasDateTo);
    }
}
