namespace Utilities.Billing.Contracts;
public interface IPaymentSystem
{
    Task AddAssetAsync(AddStellarAssetCommand command);
    Task AddPaymentAsync(AddPaymentCommand command);
    Task<string> CreateInvoiceXdr(CreateInvoiceXdrCommand command);
    Task<string> CreateWalletAsync(CreateWalletCommand command);
    Task<string> GetMasterAccountAsync();
    Task<ICollection<InvoiceInfomation>> GetInvoicesInformationAsync(IEnumerable<long> invoiceIds);
    Task DeleteCustomerAccountAsync(DeleteCustomerAccount command);
}
