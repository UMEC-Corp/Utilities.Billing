using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.InvoiceTests;
public class DeleteInvoiceRequestValidationTests
{
    [Test]
    public void Validate_Correct_DeleteInvoiceRequest()
    {
        var validator = new DeleteInvoiceRequestValidator();
        var request = new DeleteInvoiceRequest
        {
            Id = 1
        };

        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_DeleteInvoiceRequest_Id_Is_Zero()
    {
        var validator = new DeleteInvoiceRequestValidator();
        var request = new DeleteInvoiceRequest
        {
            Id = 0
        };
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }
}
