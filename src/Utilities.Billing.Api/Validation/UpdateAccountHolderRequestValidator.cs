using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class UpdateAccountHolderRequestValidator : AbstractValidator<UpdateAccountHolderRequest>
{
    public UpdateAccountHolderRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}