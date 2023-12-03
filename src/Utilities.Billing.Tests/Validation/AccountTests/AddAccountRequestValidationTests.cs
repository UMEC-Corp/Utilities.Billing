using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.AccountTests;

public class AddAccountRequestValidationTests
{
    [Test]
    public void Validate_Correct_AddAccountRequest()
    {
        var validator = new AddAccountRequestValidator();
        var request = new AddAccountRequest
        {
            AccountHolderId = 1,
            AccountTypeId = 1,
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_Incorrect_AddAccountRequest_Is_Empty()
    {
        var validator = new AddAccountRequestValidator();
        var request = new AddAccountRequest();
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }
}