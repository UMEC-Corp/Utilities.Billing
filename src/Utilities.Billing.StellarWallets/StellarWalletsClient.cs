using Utilities.Billing.Contracts;

namespace Utilities.Billing.StellarWallets;
class StellarWalletsClient : IPaymentSystem
{
    public Task AddPaymentAsync(AddPaymentCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreateWalletAsync(CreateWalletCommand command)
    {
        throw new NotImplementedException();
    }
}
