using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class GetAccountHolderRequestValidator : AbstractValidator<GetAccountHolderRequest>
{
    public GetAccountHolderRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
    }
}