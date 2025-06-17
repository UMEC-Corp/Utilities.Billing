using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

/// <summary>
/// Validator for <see cref="GetAssetRequest"/>.
/// </summary>
/// <remarks>
/// Validates that the <c>AssetId</c> field is not empty, ensuring the asset identifier is provided for retrieval.
/// </remarks>
public class GetAssetRequestValidator : AbstractValidator<GetAssetRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAssetRequestValidator"/> class.
    /// </summary>
    /// <remarks>
    /// Sets up a validation rule to ensure that AssetId is not empty.
    /// </remarks>
    public GetAssetRequestValidator()
    {
        RuleFor(x => x.AssetId).NotEmpty();
    }
}