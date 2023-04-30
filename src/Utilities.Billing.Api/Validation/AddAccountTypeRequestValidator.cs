using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class AddAccountTypeRequestValidator : AbstractValidator<AddAccountTypeRequest>
{
    public AddAccountTypeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Token).NotEmpty();
    }
}