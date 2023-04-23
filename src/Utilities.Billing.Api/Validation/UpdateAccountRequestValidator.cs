using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class UpdateAccountRequestValidator : AbstractValidator<UpdateAccountRequest>
{
    public UpdateAccountRequestValidator()
    {
        RuleFor(x => x.Account).NotNull().DependentRules(() =>
        {
            RuleFor(x => x.Account.Id).NotEmpty();
            RuleFor(x => x.Account.AccountType).Empty();
            RuleFor(x => x.Account.AccountHolder).Empty();
        });
    }
}