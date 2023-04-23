using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class UpdatePaymentRequestValidator : AbstractValidator<UpdatePaymentRequest>
{
    public UpdatePaymentRequestValidator()
    {
        RuleFor(x => x.Payment).NotNull().DependentRules(() =>
        {
            RuleFor(x => x.Payment.Id).NotEmpty();
            RuleFor(x => x.Payment.Account).Empty();
        });
    }
}