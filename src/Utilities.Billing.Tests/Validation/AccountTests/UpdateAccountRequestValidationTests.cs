﻿using Utilities.Billing.Api.Protos;
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
            Account = new Account
            {
                Id = 1
            }
        };
        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_UpdateAccountRequest_Null()
    {
        var validator = new UpdateAccountRequestValidator();
        var request = new UpdateAccountRequest();
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_UpdateAccountRequest_No_Id()
    {
        var validator = new UpdateAccountRequestValidator();
        var request = new UpdateAccountRequest
        {
            Account = new Account()
        };
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_UpdateAccountRequest_Id_Is_Zero()
    {
        var validator = new UpdateAccountRequestValidator();
        var request = new UpdateAccountRequest
        {
            Account = new Account
            {
                Id = 0
            }
        };
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }
}