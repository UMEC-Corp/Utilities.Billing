using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class AddAccountHolderRequestValidator : AbstractValidator<AddAccountHolderRequest>
{
    public AddAccountHolderRequestValidator()
    {
        RuleFor(x => x.Wallet).NotEmpty();
    }
}