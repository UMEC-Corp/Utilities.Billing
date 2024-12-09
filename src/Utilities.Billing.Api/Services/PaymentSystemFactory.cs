using Utilities.Billing.Contracts;
using Utilities.Billing.Data.Entities;
using Utilities.Billing.StellarWallets;
using Utilities.Billing.TonWallets;

namespace Utilities.Billing.Api.Services
{
    public class PaymentSystemFactory : IPaymentSystemFactory
    {
        private readonly StellarWalletsClient _stellar;
        private readonly TonWalletsClient _ton;

        public PaymentSystemFactory(StellarWalletsClient stellar, TonWalletsClient ton)
        {
            _stellar = stellar;
            _ton = ton;
        }

        public IPaymentSystem GetPaymentSystem(WalletType walletType)
        {
            switch (walletType)
            {
                case WalletType.Stellar: return _stellar;
                case WalletType.Ton: return _ton;
                default: throw new UnsupportedWalletTypeException();
            }
        }
    }
}
