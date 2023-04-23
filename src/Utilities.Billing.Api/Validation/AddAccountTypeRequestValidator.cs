using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class AddAccountTypeRequestValidator : AbstractValidator<AddAccountTypeRequest>
{
    public AddAccountTypeRequestValidator()
    {
        RuleFor(x => x.AccountType).NotNull().DependentRules(() =>
        {
            RuleFor(x => x.AccountType.HasId).Empty();
            RuleFor(x => x.AccountType.Name).NotEmpty();
            RuleFor(x => x.AccountType.Token).NotEmpty();
        });
    }
}