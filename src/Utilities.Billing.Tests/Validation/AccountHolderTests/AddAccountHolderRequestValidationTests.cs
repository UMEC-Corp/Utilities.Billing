using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.AccountHolderTests;
public class AddAccountHolderRequestValidationTests
{
    [Test]
    public void Validate_Correct_AddAccountHolderRequest()
    {
        var validator = new AddAccountHolderRequestValidator();
        var request = new AddAccountHolderRequest
        {
            Wallet = Guid.NewGuid().ToString(),
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_Incorrect_AddAccountHolderRequest_No_Data()
    {
        var validator = new AddAccountHolderRequestValidator();
        var request = new AddAccountHolderRequest();
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }
}
