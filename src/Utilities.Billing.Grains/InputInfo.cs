
namespace Utilities.Billing.Grains
{
    [GenerateSerializer]
    public class InputInfo
    {
        [Id(0)]
        public Guid AccountId { get; set; }
        [Id(1)]
        public string DeviceSerial { get; set; }
        [Id(2)]
        public decimal CurrentValue { get; set; }
        [Id(3)]
        public string AssetIssuer { get; set; }
        [Id(4)]
        public string AssetCode { get; set; }
        [Id(5)]
        public string Wallet { get; set; }
        [Id(6)]
        public Guid AssetId { get; set; }
    }
}