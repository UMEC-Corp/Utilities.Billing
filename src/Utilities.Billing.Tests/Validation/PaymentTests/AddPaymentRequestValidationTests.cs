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
            AccountId = 1,
            Amount = 1.0,
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_Incorrect_AddPaymentRequest_Is_Empty()
    {
        var validator = new AddPaymentRequestValidator();
        var request = new AddPaymentRequest();
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }
}
