

namespace Utilities.Billing.Contracts
{
    public class MakePaymentCommand
    {
        public string InputCode { get; set; }
        public double IncomingValue { get; set; }
        public string DeviceSerial { get; set; }
    }
}