namespace Utilities.Billing.Contracts;
public interface IPaymentSystem
{
    Task AddAssetAsync(AddStellarAssetCommand command);
    Task AddPaymentAsync(AddPaymentCommand command);
    Task<string> CreateWalletAsync(CreateWalletCommand command);
    Task<string> GetMasterAccountAsync();
}

public class CreateWalletCommand
{
    public bool CreateMuxed { get; set; }
}

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
