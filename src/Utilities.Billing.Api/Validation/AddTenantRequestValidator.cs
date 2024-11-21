using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation
{
    public class AddTenantRequestValidator : AbstractValidator<AddTenantRequest>
    {
        public AddTenantRequestValidator()
        {
            RuleFor(x => x.Account).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
