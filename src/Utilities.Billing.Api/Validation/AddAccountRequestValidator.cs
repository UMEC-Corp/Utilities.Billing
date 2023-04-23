using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class AddAccountRequestValidator : AbstractValidator<AddAccountRequest>
{
    public AddAccountRequestValidator()
    {
        RuleFor(x => x.Account).NotNull().DependentRules(() =>
        {
            RuleFor(x => x.Account.Id).Empty();
            RuleFor(x => x.Account.AccountHolder).Empty();
            RuleFor(x => x.Account.AccountType).Empty();
            RuleFor(x => x.Account.AccountHolderId).NotEmpty();
            RuleFor(x => x.Account.AccountTypeId).NotEmpty();
            RuleFor(x => x.Account.Wallet).Empty();
        });
    }
}