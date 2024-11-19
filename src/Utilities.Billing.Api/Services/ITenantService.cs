using Utilities.Billing.Contracts;

namespace Utilities.Billing.Api.Services;

public interface ITenantService
{
    Task<AddAssetReply> AddAsset(AddAssetCommand command);
    Task<GetAssetReply> GetAsset(GetAssetCommand command);
    Task UpdateAsset(UpdateAssetCommand command);
    Task<CreateCustomerAccountReply> CreateCustomerAccount(CreateCustomerAccountCommand command);
    Task<GetCustomerAccountReply> GetCustomerAccount(GetCustomerAccountCommand command);
    Task<CreateInvoiceReply> CreateInvoice(CreateInvoiceCommand command);
    Task<ListInvoicesReply> ListInvoices(ListInvoicesCommand command);
    Task<AddTenantReply> AddTenant(AddTenantCommand command);
    Task UpdateTenant(UpdateTenantCommand updateTenantCommand);
    Task DeleteCustomerAccount(DeleteCustomerAccountCommand command);
    Task<Page<AccountItem>> ListCustomerAccounts(ListCustomerAccountsCommand command);
    Task<Page<AssetItem>> ListAssets(ListAssetsCommand command);
}