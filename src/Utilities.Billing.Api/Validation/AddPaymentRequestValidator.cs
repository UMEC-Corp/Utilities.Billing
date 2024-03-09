using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class AddPaymentRequestValidator : AbstractValidator<AddPaymentRequest>
{
    public AddPaymentRequestValidator()
    {
        var now = (ulong)DateTime.UtcNow.ToUnixTimeSeconds();

        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.Date).GreaterThanOrEqualTo(now).When(request => request.HasDate);
        RuleFor(x => x.Amount).NotEmpty().GreaterThan(.0);
    }
}