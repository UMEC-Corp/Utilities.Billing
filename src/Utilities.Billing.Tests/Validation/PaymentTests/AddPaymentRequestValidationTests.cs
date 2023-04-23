using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.PaymentTests;
public class AddPaymentRequestValidationTests
{
    [Test]
    public void Validate_Correct_AddPaymentRequest()
    {
        var validator = new AddPaymentRequestValidator();
        var request = new AddPaymentRequest
        {
            Payment = new Payment
            {
                AccountId = 1,
                Amount = 1,
            }
        };
        var result = validator.Validate(request);
        Assert.IsTrue(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddPaymentRequest_With_Id()
    {
        var validator = new AddPaymentRequestValidator();
        var request = new AddPaymentRequest
        {
            Payment = new Payment
            {
                Id = 1,
            }
        };
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddPaymentRequest_No_Data()
    {
        var validator = new AddPaymentRequestValidator();
        var request = new AddPaymentRequest();
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
    }
}
