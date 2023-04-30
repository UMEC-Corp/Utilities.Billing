using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.PaymentTests;
public class UpdatePaymentRequestValidationTests
{
    [Test]
    public void Validate_Correct_UpdatePaymentRequest()
    {
        var validator = new UpdatePaymentRequestValidator();
        var request = new UpdatePaymentRequest
        {

            Id = 1,
        };

        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_UpdatePaymentRequest_Id_Is_Empty()
    {
        var validator = new UpdatePaymentRequestValidator();
        var request = new UpdatePaymentRequest
        {
            Id = 0,
        };
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_UpdatePaymentRequest_Is_Empty()
    {
        var validator = new UpdatePaymentRequestValidator();
        var request = new UpdatePaymentRequest();
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }
}
