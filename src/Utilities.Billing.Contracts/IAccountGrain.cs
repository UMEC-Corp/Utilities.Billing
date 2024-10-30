namespace Utilities.Billing.Contracts
{
    public interface IDeviceGrain : IGrainWithStringKey
    {
        Task<MakePaymentReply> MakePaymentAsync(MakePaymentCommand command);
    }
}