using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class AddAccountRequestValidator : AbstractValidator<AddAccountRequest>
{
    public AddAccountRequestValidator()
    {
        RuleFor(x => x.AccountHolderId).NotEmpty();
        RuleFor(x => x.AccountTypeId).NotEmpty();
    }
}