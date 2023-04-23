using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

public class GetInvoiceRequestValidator : AbstractValidator<GetInvoiceRequest>
{
    public GetInvoiceRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
    }
}