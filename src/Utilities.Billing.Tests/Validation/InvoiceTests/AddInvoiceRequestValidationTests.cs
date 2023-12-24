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
            AccountId = 1,
            Amount = 1,
        };
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_Incorrect_AddInvoiceRequest_Is_Empty()
    {
        var validator = new AddInvoiceRequestValidator();
        var request = new AddInvoiceRequest();
        var result = validator.Validate(request);
        Assert.That(result.IsValid, Is.False);
    }
}
