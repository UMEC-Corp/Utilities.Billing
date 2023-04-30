using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class UpdateInvoiceRequestValidator : AbstractValidator<UpdateInvoiceRequest>
{
    public UpdateInvoiceRequestValidator()
    {
        var now = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(.0).When(x => x.HasAmount);
        RuleFor(x => x.AccountId).NotEmpty().When(x => x.HasAccountId);
        RuleFor(x => x.Date).GreaterThanOrEqualTo(now).When(request => request.HasDate);
        RuleFor(x => x.DateTo).GreaterThanOrEqualTo(now).When(request => request.HasDateTo);
        RuleFor(x => x.DateTo).GreaterThan(x => x.Date).When(request => request.HasDateTo);
    }
}