using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class DeleteAccountTypeRequestValidator : AbstractValidator<DeleteAccountTypeRequest>
{
    public DeleteAccountTypeRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
    }
}