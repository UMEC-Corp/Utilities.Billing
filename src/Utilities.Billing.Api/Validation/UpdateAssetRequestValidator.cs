using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

/// <summary>
/// Validator for <see cref="UpdateAssetRequest"/>.
/// </summary>
/// <remarks>
/// Validates that the following fields are not empty:
/// <list type="bullet">
///   <item><description><c>AssetId</c>: The identifier of the asset to update.</description></item>
///   <item><description><c>ModelCodes</c>: The collection of model codes to associate with the asset.</description></item>
/// </list>
/// </remarks>
public class UpdateAssetRequestValidator : AbstractValidator<UpdateAssetRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAssetRequestValidator"/> class.
    /// </summary>
    /// <remarks>
    /// Sets up validation rules to ensure that AssetId and ModelCodes are not empty.
    /// </remarks>
    public UpdateAssetRequestValidator()
    {
        RuleFor(x => x.AssetId).NotEmpty();
        RuleFor(x => x.ModelCodes).NotEmpty();
    }
}