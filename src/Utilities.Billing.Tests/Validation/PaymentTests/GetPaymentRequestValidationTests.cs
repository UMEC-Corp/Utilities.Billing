using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.PaymentTests;
public class GetPaymentRequestValidationTests
{
    [Test]
    public void Validate_Correct_GetAccountRequest()
    {
        var validator = new GetPaymentRequestValidator();
        var request = new GetPaymentRequest
        {
            Id = 1
        };

        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_GetAccountRequest_Id_Is_Zero()
    {
        var validator = new GetPaymentRequestValidator();
        var request = new GetPaymentRequest
        {
            Id = 0
        };
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }

}
