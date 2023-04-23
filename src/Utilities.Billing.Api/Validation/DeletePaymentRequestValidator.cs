using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class DeletePaymentRequestValidator : AbstractValidator<DeletePaymentRequest>
{
    public DeletePaymentRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
    }
}