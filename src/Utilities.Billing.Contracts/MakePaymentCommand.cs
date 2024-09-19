
namespace Utilities.Billing.Contracts
{
    [GenerateSerializer]
    public class MakePaymentCommand
    {
        [Id(0)]
        public string? InputCode { get; set; }
        [Id(1)]
        public byte[] IncomingValue { get; set; }
    }
}