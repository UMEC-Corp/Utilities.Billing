using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.AccountTypeTests;
public class UpdateAccountTypeRequestValidationTests
{
    [Test]
    public void Validate_Correct_UpdateAccountTypeRequest()
    {
        var validator = new UpdateAccountTypeRequestValidator();
        var request = new UpdateAccountTypeRequest
        {
            AccountType = new AccountType
            {
                Id = 1,
            }
        };

        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_UpdateAccountTypeRequest_Id_Is_Zero()
    {
        var validator = new UpdateAccountTypeRequestValidator();
        var request = new UpdateAccountTypeRequest
        {
            AccountType = new AccountType
            {
                Id = 0,
            }
        };
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_UpdateAccountTypeRequest_Id_Is_Empty()
    {
        var validator = new UpdateAccountTypeRequestValidator();
        var request = new UpdateAccountTypeRequest
        {
            AccountType = new AccountType
            {
            }
        };
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_UpdateAccountTypeRequest_Empty()
    {
        var validator = new UpdateAccountTypeRequestValidator();
        var request = new UpdateAccountTypeRequest();
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }
}
