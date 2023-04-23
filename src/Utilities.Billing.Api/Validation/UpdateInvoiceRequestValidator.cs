using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class UpdateInvoiceRequestValidator : AbstractValidator<UpdateInvoiceRequest>
{
    public UpdateInvoiceRequestValidator()
    {
        RuleFor(x => x.Invoice).NotNull().DependentRules(() =>
        {
            RuleFor(x => x.Invoice.Id).NotEmpty();
            RuleFor(x => x.Invoice.Account).Empty();
        });
    }
}