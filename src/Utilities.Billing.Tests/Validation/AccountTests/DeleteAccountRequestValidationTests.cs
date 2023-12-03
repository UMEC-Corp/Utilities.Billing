using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.AccountTests;

public class DeleteAccountRequestValidationTests
{
    [Test]
    public void Validate_Correct_DeleteAccountRequest()
    {
        var validator = new DeleteAccountRequestValidator();
        var request = new DeleteAccountRequest
        {
            Id = 1
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_Incorrect_DeleteAccountRequest()
    {
        var validator = new DeleteAccountRequestValidator();
        var request = new DeleteAccountRequest
        {
            Id = 0
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }
}