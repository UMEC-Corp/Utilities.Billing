namespace Utilities.Billing.Contracts;
public interface ITenantGrain : IGrainWithGuidKey
{
    Task<long> AddAccountTypeAsync(AddAccountTypeCommand command);
    Task DeleteAccountTypeAsync(DeleteAccountTypeCommand command);
    Task UpdateAccountTypeAsync(UpdateAccountTypeCommand command);
    Task<Page<AccountTypeItem>> GetAccountTypesAsync(GetAccountTypesQuery query);
    Task<AccountTypeItem> GetAccountTypeAsync(GetAccountTypeQuery query);
    Task<AddPaymentsReply> AddPaymentsForInvoicesAsync(AddPaymentsForInvoicesCommand command);
    Task<AddInvoicesReply> AddInvoicesAsync(AddInvoicesCommand addInvoicesCommand);

    Task<AddAssetReply> AddAsset(AddAssetCommand command);
    Task<GetAssetReply> GetAsset(GetAssetCommand command);
    Task UpdateAsset(UpdateAssetCommand command);
    Task<CreateCustomerAccountReply> CreateCustomerAccount(CreateCustomerAccountCommand command);
    Task<GetCustomerAccountReply> GetCustomerAccount(GetCustomerAccountCommand command);
    Task<CreateInvoiceReply> CreateInvoice(CreateInvoiceCommand command);
    Task<ListInvoicesReply> ListInvoices(ListInvoicesCommand command);
    Task<AddTenantReply> AddTenant(AddTenantCommand command);
}
