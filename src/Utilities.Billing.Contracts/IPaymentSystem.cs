namespace Utilities.Billing.Contracts;
public interface IPaymentSystem
{
    Task AddPaymentAsync(AddPaymentCommand command);
    Task<string> CreateWalletAsync(CreateWalletCommand command);
}

public record CreateWalletCommand(
    Guid TenantId,
    string Token
);

public record AddPaymentCommand
(
    Guid TenantId,
    long PaymentId,
    string BuyToken,
    decimal BuyTokenAmount,
    string SellCurrency,
    decimal SellCurrencyAmount,
    string BuyerCurrencyWallet,
    string SellerCurrencyWallet,
    string BuyerTokenWallet,
    string SellerTokenWallet
);
