using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.AccountHolderTests;
public class UpdateAccountHolderRequestValidationTests
{
    [Test]
    public void Validate_Correct_UpdateAccountHolderRequest()
    {
        var validator = new UpdateAccountHolderRequestValidator();
        var request = new UpdateAccountHolderRequest
        {
            Id = 1,
        };

        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_Incorrect_UpdateAccountHolderRequest_Id_Is_Zero()
    {
        var validator = new UpdateAccountHolderRequestValidator();
        var request = new UpdateAccountHolderRequest
        {
            Id = 0,
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_Incorrect_UpdateAccountHolderRequest_Id_Is_Empty()
    {
        var validator = new UpdateAccountHolderRequestValidator();
        var request = new UpdateAccountHolderRequest
        {
            Id = 0,
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_Incorrect_UpdateAccountHolderRequest_Empty()
    {
        var validator = new UpdateAccountHolderRequestValidator();
        var request = new UpdateAccountHolderRequest();
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }
}
