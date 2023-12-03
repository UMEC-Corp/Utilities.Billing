using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.AccountHolderTests;
public class GetAccountHolderRequestValidationTests
{
    [Test]
    public void Validate_Correct_GetAccountRequest()
    {
        var validator = new GetAccountHolderRequestValidator();
        var request = new GetAccountHolderRequest
        {
            Id = 1
        };

        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_Incorrect_GetAccountRequest_Id_Is_Zero()
    {
        var validator = new GetAccountHolderRequestValidator();
        var request = new GetAccountHolderRequest
        {
            Id = 0
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_Incorrect_GetAccountRequest_Id_Is_Empty()
    {
        var validator = new GetAccountHolderRequestValidator();
        var request = new GetAccountHolderRequest();
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }
}
