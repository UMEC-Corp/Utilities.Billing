namespace Utilities.Billing.Contracts
{
    public interface IDeviceGrain : IGrainWithStringKey
    {
        Task<MakePaymentReply> MakePayment(MakePaymentCommand command);
    }
}