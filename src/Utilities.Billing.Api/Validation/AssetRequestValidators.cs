using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

/// <summary>
/// Validator for <see cref="AddAssetRequest"/>.
/// </summary>
/// <remarks>
/// Validates that the following fields are not empty:
/// <list type="bullet">
///   <item><description><c>AssetCode</c>: The code of the asset to add.</description></item>
///   <item><description><c>Issuer</c>: The issuer account for the asset.</description></item>
///   <item><description><c>ModelCodes</c>: The collection of model codes associated with the asset.</description></item>
/// </list>
/// </remarks>
public class AddAssetRequestValidator : AbstractValidator<AddAssetRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddAssetRequestValidator"/> class.
    /// </summary>
    /// <remarks>
    /// Sets up validation rules to ensure that AssetCode, Issuer, and ModelCodes are not empty.
    /// </remarks>
    public AddAssetRequestValidator()
    {
        RuleFor(x => x.AssetCode).NotEmpty();
        RuleFor(x => x.Issuer).NotEmpty();
        RuleFor(x => x.ModelCodes).NotEmpty();
    }
}