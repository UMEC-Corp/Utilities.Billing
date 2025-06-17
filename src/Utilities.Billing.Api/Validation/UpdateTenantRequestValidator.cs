using FluentValidation;
using Utilities.Billing.Api.Protos;

namespace Utilities.Billing.Api.Validation;


/// <summary>
/// Validator for <see cref="UpdateTenantRequest"/>.
/// </summary>
/// <remarks>
/// This validator ensures that the following fields of <see cref="UpdateTenantRequest"/> are not empty:
/// <list type="bullet">
///   <item><description><c>Id</c>: The unique identifier of the tenant to update. Must be provided.</description></item>
///   <item><description><c>Account</c>: The account associated with the tenant. Must be provided.</description></item>
///   <item><description><c>Name</c>: The name of the tenant. Must be provided.</description></item>
/// </list>
/// If any of these fields are empty, validation will fail and an appropriate error message will be returned.
/// </remarks>
public class UpdateTenantRequestValidator : AbstractValidator<UpdateTenantRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTenantRequestValidator"/> class.
    /// </summary>
    /// <remarks>
    /// Sets up validation rules to ensure that Id, Account, and Name are not empty.
    /// </remarks>
    public UpdateTenantRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Account).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}
