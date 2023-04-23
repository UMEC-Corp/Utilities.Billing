using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class DeleteAccountHolderRequestValidator : AbstractValidator<DeleteAccountHolderRequest>
{
    public DeleteAccountHolderRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}