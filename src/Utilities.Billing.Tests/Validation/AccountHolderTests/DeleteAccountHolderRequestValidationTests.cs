using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.AccountHolderTests;
public class DeleteAccountHolderRequestValidationTests
{
    [Test]
    public void Validate_Correct_DeleteAccountHolderRequest()
    {
        var validator = new DeleteAccountHolderRequestValidator();
        var request = new DeleteAccountHolderRequest
        {
            Id = 1
        };

        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_Incorrect_DeleteAccountHolderRequest_Id_Is_Zero()
    {
        var validator = new DeleteAccountHolderRequestValidator();
        var request = new DeleteAccountHolderRequest
        {
            Id = 0
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_Incorrect_DeleteAccountHolderRequest_Id_Is_Empty()
    {
        var validator = new DeleteAccountHolderRequestValidator();
        var request = new DeleteAccountHolderRequest();
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }
}
