﻿using Utilities.Billing.Api.Protos;
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
            AccountHolder = new AccountHolder
            {
                Wallet = Guid.NewGuid().ToString(),
            }
        };
        var result = validator.Validate(request);
        Assert.IsTrue(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddAccountHolderRequest_No_Data()
    {
        var validator = new AddAccountHolderRequestValidator();
        var request = new AddAccountHolderRequest();
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
    }
}
