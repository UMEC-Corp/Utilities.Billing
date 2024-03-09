using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class AddInvoiceRequestValidator : AbstractValidator<AddInvoiceRequest>
{
    public AddInvoiceRequestValidator()
    {
        var now = (ulong)DateTime.UtcNow.ToUnixTimeSeconds();

        RuleFor(x => x.Amount).GreaterThan(.0);
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.Date).GreaterThanOrEqualTo(now).When(request => request.HasDate);
        RuleFor(x => x.DateTo).GreaterThanOrEqualTo(now).When(request => request.HasDateTo);
        RuleFor(x => x.DateTo).GreaterThan(x => x.Date).When(request => request.HasDateTo);
    }
}