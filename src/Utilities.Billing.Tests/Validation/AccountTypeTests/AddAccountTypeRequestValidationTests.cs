using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.AccountTypeTests;
public class AddAccountTypeRequestValidationTests
{
    [Test]
    public void Validate_Correct_AddAccountTypeRequest()
    {
        var validator = new AddAccountTypeRequestValidator();
        var request = new AddAccountTypeRequest
        {
            Name = Guid.NewGuid().ToString(),
            Token = Guid.NewGuid().ToString(),
        };
        var result = validator.Validate(request);
        Assert.IsTrue(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddAccountTypeRequest_Name_Is_Empty()
    {
        var validator = new AddAccountTypeRequestValidator();
        var request = new AddAccountTypeRequest
        {
            Token = Guid.NewGuid().ToString(),
        };
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddAccountTypeRequest_Token_Is_Empty()
    {
        var validator = new AddAccountTypeRequestValidator();
        var request = new AddAccountTypeRequest
        {
            Name = Guid.NewGuid().ToString(),
        };
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddAccountTypeRequest_Is_Empty()
    {
        var validator = new AddAccountTypeRequestValidator();
        var request = new AddAccountTypeRequest();
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
    }
}
