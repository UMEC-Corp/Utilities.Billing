namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class ListInvoicesReply
{
    [Id(0)]
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

    [GenerateSerializer]
    public class InvoiceItem
    {
        [Id(0)]
        public string TransactionId { get; set; }
        [Id(1)]
        public string Amount { get; set; }
        [Id(2)]
        public bool Processed { get; set; }
        [Id(3)]
        public string Xdr { get; set; }
    }
}
