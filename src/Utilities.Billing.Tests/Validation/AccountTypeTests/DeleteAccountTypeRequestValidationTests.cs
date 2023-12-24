using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.AccountTypeTests;
public class DeleteAccountTypeRequestValidationTests
{
    [Test]
    public void Validate_Correct_DeleteAccountTypeRequest()
    {
        var validator = new DeleteAccountTypeRequestValidator();
        var request = new DeleteAccountTypeRequest
        {
            Id = 1
        };

        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_Incorrect_DeleteAccountTypeRequest_Id_Is_Zero()
    {
        var validator = new DeleteAccountTypeRequestValidator();
        var request = new DeleteAccountTypeRequest
        {
            Id = 0
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }
}
