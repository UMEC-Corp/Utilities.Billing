using Utilities.Billing.Contracts;
using Utilities.Billing.Data.Entities;

namespace Utilities.Billing.Api.Services
{
    public interface IPaymentSystemFactory
    {
        IPaymentSystem GetPaymentSystem(WalletType walletType);
    }
}