
namespace Utilities.Billing.Contracts
{
    public class MakePaymentCommand
    {
        public string? InputCode { get; set; }
        public byte[] IncomingValue { get; set; }
    }
}