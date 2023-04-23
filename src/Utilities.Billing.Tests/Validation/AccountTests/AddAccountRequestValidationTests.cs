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
            Account = new Account
            {
                AccountHolderId = 1,
                AccountTypeId = 1,
            }
        };
        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddAccountRequest_Account_Is_Null()
    {
        var validator = new AddAccountRequestValidator();
        var request = new AddAccountRequest();
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddAccountRequest_Id_Is_Specified()
    {
        var validator = new AddAccountRequestValidator();
        var request = new AddAccountRequest
        {
            Account = new Account
            {
                Id = 1
            }
        };
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }
}