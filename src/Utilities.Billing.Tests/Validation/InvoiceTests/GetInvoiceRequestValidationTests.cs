using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.InvoiceTests;
public class GetInvoiceRequestValidationTests
{
    [Test]
    public void Validate_Correct_GetAccountRequest()
    {
        var validator = new GetInvoiceRequestValidator();
        var request = new GetInvoiceRequest
        {
            Id = 1
        };

        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_Incorrect_GetAccountRequest_Id_Is_Zero()
    {
        var validator = new GetInvoiceRequestValidator();
        var request = new GetInvoiceRequest
        {
            Id = 0
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }

}
