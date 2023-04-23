using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class DeleteInvoiceRequestValidator : AbstractValidator<DeleteInvoiceRequest>
{
    public DeleteInvoiceRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
    }
}