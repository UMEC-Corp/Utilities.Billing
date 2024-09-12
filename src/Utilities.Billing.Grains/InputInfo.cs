
namespace Utilities.Billing.Grains
{
    internal class InputInfo
    {
        public Guid AccountId { get; set; }
        public string DeviceSerial { get; set; }
        public decimal CurrentValue { get; set; }
        public string AssetIssuer { get; set; }
        public string AssetCode { get; set; }
        public string Wallet { get; set; }
        public Guid AssetId { get; set; }
    }
}