using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation
{
    public class UpdateTenantRequestValidator : AbstractValidator<UpdateTenantRequest>
    {
        public UpdateTenantRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Account).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
