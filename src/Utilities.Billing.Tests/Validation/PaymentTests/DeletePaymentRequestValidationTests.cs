using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.PaymentTests;
public class DeletePaymentRequestValidationTests
{
    [Test]
    public void Validate_Correct_DeletePaymentRequest()
    {
        var validator = new DeletePaymentRequestValidator();
        var request = new DeletePaymentRequest
        {
            Id = 1
        };

        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_Incorrect_DeletePaymentRequest_Id_Is_Zero()
    {
        var validator = new DeletePaymentRequestValidator();
        var request = new DeletePaymentRequest
        {
            Id = 0
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }
}
