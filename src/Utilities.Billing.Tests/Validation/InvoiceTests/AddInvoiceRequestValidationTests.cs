using Utilities.Billing.Api.Protos;
using Utilities.Billing.Api.Validation;

namespace Utilities.Billing.Tests.Validation.InvoiceTests;
public class AddInvoiceRequestValidationTests
{
    [Test]
    public void Validate_Correct_AddInvoiceRequest()
    {
        var validator = new AddInvoiceRequestValidator();
        var request = new AddInvoiceRequest
        {
            Invoice = new Invoice
            {
                AccountId = 1,
                Amount = 1,
            }
        };
        var result = validator.Validate(request);
        Assert.IsTrue(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddInvoiceRequest_With_Id()
    {
        var validator = new AddInvoiceRequestValidator();
        var request = new AddInvoiceRequest
        {
            Invoice = new Invoice
            {
                Id = 1,
            }
        };
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_Incorrect_AddInvoiceRequest_No_Data()
    {
        var validator = new AddInvoiceRequestValidator();
        var request = new AddInvoiceRequest();
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
    }
}
