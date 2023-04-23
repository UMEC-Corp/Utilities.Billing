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
            AccountType = new AccountType
            {
                Name = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString(),
            }
        };
        var result = validator.Validate(request);
        Assert.IsTrue(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddAccountTypeRequest_With_Id()
    {
        var validator = new AddAccountTypeRequestValidator();
        var request = new AddAccountTypeRequest
        {
            AccountType = new AccountType
            {
                Id = 1,
            }
        };
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddAccountTypeRequest_Without_Name()
    {
        var validator = new AddAccountTypeRequestValidator();
        var request = new AddAccountTypeRequest
        {
            AccountType = new AccountType
            {
                Token = Guid.NewGuid().ToString(),
            }
        };
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddAccountTypeRequest_Without_Token()
    {
        var validator = new AddAccountTypeRequestValidator();
        var request = new AddAccountTypeRequest
        {
            AccountType = new AccountType
            {
                Name = Guid.NewGuid().ToString(),
            }
        };
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddAccountTypeRequest_No_Data()
    {
        var validator = new AddAccountTypeRequestValidator();
        var request = new AddAccountTypeRequest();
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
    }
}
