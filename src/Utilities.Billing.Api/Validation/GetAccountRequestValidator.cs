using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class GetAccountRequestValidator : AbstractValidator<GetAccountRequest>
{
    public GetAccountRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
    }
}