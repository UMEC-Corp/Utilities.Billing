using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation
{
    public class AddAssetRequestValidator : AbstractValidator<AddAssetRequest>
    {
        public AddAssetRequestValidator()
        {
            RuleFor(x => x.AssetCode).NotEmpty();
            RuleFor(x => x.Issuer).NotEmpty();
            RuleFor(x => x.ModelCodes).NotEmpty();
        }
    }

    public class GetAssetRequestValidator : AbstractValidator<GetAssetRequest>
    {
        public GetAssetRequestValidator()
        {
            RuleFor(x => x.AssetId).NotEmpty();
        }
    }

    public class UpdateAssetRequestValidator : AbstractValidator<UpdateAssetRequest>
    {
        public UpdateAssetRequestValidator()
        {
            RuleFor(x => x.AssetId).NotEmpty();
            RuleFor(x => x.ModelCodes).NotEmpty();
        }
    }
    
}
