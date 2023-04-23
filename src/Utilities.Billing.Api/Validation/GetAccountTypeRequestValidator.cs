using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class GetAccountTypeRequestValidator : AbstractValidator<GetAccountTypeRequest>
{
    public GetAccountTypeRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
    }
}
