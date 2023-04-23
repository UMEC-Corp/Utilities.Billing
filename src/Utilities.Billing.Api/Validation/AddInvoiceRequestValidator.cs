using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class AddInvoiceRequestValidator : AbstractValidator<AddInvoiceRequest>
{
    public AddInvoiceRequestValidator()
    {
        RuleFor(x => x.Invoice).NotNull().DependentRules(() =>
        {
            RuleFor(x => x.Invoice.Id).Empty();
            RuleFor(x => x.Invoice.Amount).NotEmpty();
            RuleFor(x => x.Invoice.AccountId).NotEmpty();
            RuleFor(x => x.Invoice.Account).Empty();
            RuleFor(x => x.Invoice.Date).Empty();
            RuleFor(x => x.Invoice.AmountPaid).Empty();
        });
    }
}