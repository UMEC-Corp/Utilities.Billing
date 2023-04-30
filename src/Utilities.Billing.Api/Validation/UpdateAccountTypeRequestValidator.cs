using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class UpdateAccountTypeRequestValidator : AbstractValidator<UpdateAccountTypeRequest>
{
    public UpdateAccountTypeRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}