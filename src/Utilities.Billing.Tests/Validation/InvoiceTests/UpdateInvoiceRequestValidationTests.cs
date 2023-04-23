using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.InvoiceTests;
public class UpdateInvoiceRequestValidationTests
{
    [Test]
    public void Validate_Correct_UpdateInvoiceRequest()
    {
        var validator = new UpdateInvoiceRequestValidator();
        var request = new UpdateInvoiceRequest
        {
            Invoice = new Invoice
            {
                Id = 1,
            }
        };

        var result = validator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_UpdateInvoiceRequest_Id_Is_Zero()
    {
        var validator = new UpdateInvoiceRequestValidator();
        var request = new UpdateInvoiceRequest
        {
            Invoice = new Invoice
            {
                Id = 0,
            }
        };
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_UpdateInvoiceRequest_Id_Is_Empty()
    {
        var validator = new UpdateInvoiceRequestValidator();
        var request = new UpdateInvoiceRequest
        {
            Invoice = new Invoice
            {
            }
        };
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_UpdateInvoiceRequest_Empty()
    {
        var validator = new UpdateInvoiceRequestValidator();
        var request = new UpdateInvoiceRequest();
        var result = validator.Validate(request);
        Assert.False(result.IsValid);
    }
}
