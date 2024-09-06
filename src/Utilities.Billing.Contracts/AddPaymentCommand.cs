namespace Utilities.Billing.Contracts;

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
