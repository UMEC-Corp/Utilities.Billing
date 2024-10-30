namespace Utilities.Billing.Contracts
{
    [GenerateSerializer]
    public class MakePaymentReply
    {
        public static readonly MakePaymentReply Skipped = new ();
    }
}