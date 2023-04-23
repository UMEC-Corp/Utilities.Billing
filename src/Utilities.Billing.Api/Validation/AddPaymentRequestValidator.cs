using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class AddPaymentRequestValidator : AbstractValidator<AddPaymentRequest>
{
    public AddPaymentRequestValidator()
    {
        RuleFor(x => x.Payment).NotNull().DependentRules(() =>
        {
            RuleFor(x => x.Payment.Id).Empty();
            RuleFor(x => x.Payment.AccountId).NotEmpty();
            RuleFor(x => x.Payment.Account).Empty();
            RuleFor(x => x.Payment.Date).Empty();
            RuleFor(x => x.Payment.Amount).NotEmpty();
            RuleFor(x => x.Payment.Status).Empty();
            RuleFor(x => x.Payment.Transaction).Empty();
        });
    }
}