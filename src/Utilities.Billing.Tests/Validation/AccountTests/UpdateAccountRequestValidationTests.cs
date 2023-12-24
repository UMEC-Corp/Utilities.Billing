using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.AccountTests;

public class UpdateAccountRequestValidationTests
{
    [Test]
    public void Validate_Correct_UpdateAccountRequest()
    {
        var validator = new UpdateAccountRequestValidator();
        var request = new UpdateAccountRequest
        {
            Id = 1
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_Incorrect_UpdateAccountRequest_Is_Empty()
    {
        var validator = new UpdateAccountRequestValidator();
        var request = new UpdateAccountRequest();
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }


    [Test]
    public void Validate_Incorrect_UpdateAccountRequest_Id_Is_Empty()
    {
        var validator = new UpdateAccountRequestValidator();
        var request = new UpdateAccountRequest
        {
            Id = 0
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }
}