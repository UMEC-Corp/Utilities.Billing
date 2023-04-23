using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class GetPaymentRequestValidator : AbstractValidator<GetPaymentRequest>
{
    public GetPaymentRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
    }
}