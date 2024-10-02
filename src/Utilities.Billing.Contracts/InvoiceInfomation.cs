namespace Utilities.Billing.Contracts
{
    public class InvoiceInfomation
    {
        public long Id { get; set; }
        public PaymentSystemTransactionStatus Status { get; set; }
    }
}