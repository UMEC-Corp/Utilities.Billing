namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class AddInvoicesCommand
{
    [Id(0)]
    public List<Item> Items { get; } = new();

    [GenerateSerializer]
    public class Item
    {
        [Id(0)]
        public long AccountId { get; set; }
        [Id(1)]
        public decimal Amount { get; set; }
        [Id(2)]
        public DateTime? Date { get; set; }
        [Id(3)]
        public DateTime? DateTo { get; set; }
    }
}
