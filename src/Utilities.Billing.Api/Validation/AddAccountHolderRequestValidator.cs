using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class AddAccountHolderRequestValidator : AbstractValidator<AddAccountHolderRequest>
{
    public AddAccountHolderRequestValidator()
    {
        RuleFor(x => x.AccountHolder).NotNull().DependentRules(() =>
        {
            RuleFor(x => x.AccountHolder.Id).Empty();
            RuleFor(x => x.AccountHolder.Wallet).NotEmpty();
        });
    }
}