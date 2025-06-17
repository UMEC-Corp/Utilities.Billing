using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;

/// <summary>
/// Validator for <see cref="UpdatePaymentRequest"/>.
/// </summary>
/// <remarks>
/// This validator ensures that the <c>Id</c> property of <see cref="UpdatePaymentRequest"/> is not empty.
/// The <c>Id</c> field represents the unique identifier of the payment to update and is required for the update operation.
/// If <c>Id</c> is empty, validation will fail and an error will be returned.
/// </remarks>
public class UpdatePaymentRequestValidator : AbstractValidator<UpdatePaymentRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePaymentRequestValidator"/> class.
    /// </summary>
    /// <remarks>
    /// Sets up a validation rule to ensure that the <c>Id</c> property is not empty.
    /// </remarks>
    public UpdatePaymentRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
