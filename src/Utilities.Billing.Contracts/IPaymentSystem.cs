namespace Utilities.Billing.Contracts;
public interface IPaymentSystem
{
    Task<string> AddAsset(AddStellarAssetCommand command);
    Task AddPaymentAsync(AddPaymentCommand command);
    Task<string> CreateWalletAsync(CreateWalletCommand command);
    Task<string> GetMasterAccount();
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

public class AddStellarAssetCommand
{
    public string AssetCode { get; set; }
    public string IssuerAccountId { get; set; }
    public string ReceiverAccountId { get; set; }
}
