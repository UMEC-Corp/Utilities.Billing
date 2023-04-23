using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class UpdateAccountHolderRequestValidator : AbstractValidator<UpdateAccountHolderRequest>
{
    public UpdateAccountHolderRequestValidator()
    {
        RuleFor(x => x.AccountHolder).NotNull().DependentRules(() =>
        {
            RuleFor(x => x.AccountHolder.Id).NotEmpty();
        });

    }
}