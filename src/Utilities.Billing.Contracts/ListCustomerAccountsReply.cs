
namespace Utilities.Billing.Contracts
{
    [GenerateSerializer]
    public class ListCustomerAccountsReply
    {
        [Id(0)]
        public IList<Item> Items { get; set; } = new List<Item>();

        [GenerateSerializer]
        public class Item
        {
            [Id(0)]
            public Guid Id { get; set; }
            [Id(1)]
            public string Wallet { get; set; }
            [Id(2)]
            public string AssetCode { get; set; }
            [Id(3)]
            public string DeviceSerial { get; set; }
            [Id(4)]
            public string InputCode { get; set; }
            [Id(5)]
            public ContractsAccountState State { get; set; }
        }
    }
}