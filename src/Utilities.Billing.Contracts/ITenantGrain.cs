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
}

[GenerateSerializer]
public class CreateInvoiceReply
{
    [Id(0)]
    public string Xdr { get; set; }
}

[GenerateSerializer]
public class CreateInvoiceCommand
{
    [Id(0)]
    public string CustomerAccountId { get; set; }
    [Id(1)]
    public string PayerAccount { get; set; }
    [Id(2)]
    public string Amount { get; set; }
}

[GenerateSerializer]
public class GetCustomerAccountReply
{
    [Id(0)]
    public Guid Id { get; set; }
    [Id(1)]
    public string Wallet { get; set; }
    [Id(2)]
    public Guid AssetId { get; set; }
    [Id(3)]
    public string AssetCode { get; set; }
    [Id(4)]
    public string AssetIssuer { get; set; }
    [Id(5)]
    public string MasterAccount { get; set; }
}

[GenerateSerializer]
public class GetCustomerAccountCommand
{
    [Id(0)]
    public string CustomerAccountId { get; set; }
}

[GenerateSerializer]
public class CreateCustomerAccountReply
{
    [Id(0)]
    public Guid AccountId { get; set; }
}

[GenerateSerializer]
public class CreateCustomerAccountCommand
{
    [Id(0)]
    public string AssetId { get; set; }
    [Id(1)]
    public string ControllerSerial { get; set; }
    [Id(2)]
    public bool CreateMuxed { get; set; }
    [Id(3)]
    public string MeterNumber { get; set; }
}

[GenerateSerializer]
public class UpdateAssetCommand
{
    [Id(0)]
    public string Id { get; set; }
    [Id(1)]
    public ICollection<string> ModelCodes { get; set; } = new List<string>();
}

[GenerateSerializer]
public class GetAssetReply
{
    [Id(0)]
    public Guid Id { get; set; }
    [Id(1)]
    public string Code { get; set; }
    [Id(2)]
    public string IssuerAccount { get; set; }
    [Id(3)]
    public string MasterAccount { get; set; }
    [Id(4)]
    public ICollection<string> ModelCodes { get; set; } = new List<string>();

}

[GenerateSerializer]
public class GetAssetCommand
{
    [Id(0)]
    public string Id { get; set; }
}

[GenerateSerializer]
public class AddAssetReply
{
    [Id(0)]
    public Guid Id { get; set; }
}

[GenerateSerializer]
public class AddAssetCommand
{
    [Id(0)]
    public string AssetCode { get; set; }
    [Id(1)]
    public string Issuer { get; set; }
    [Id(2)]
    public ICollection<string> ModelCodes { get; set; } = new List<string>();
}

[GenerateSerializer]
public class AddInvoicesReply
{
    [Id(0)]
    public List<long> InvoiceIds { get; } = new();
}

[GenerateSerializer]
public class AddInvoicesCommand
{
    [Id(0)]
    public List<Item> Items { get; } = new();

    [GenerateSerializer]
    public class Item
    {
        [Id(0)]
        public long AccountId { get; set; }
        [Id(1)]
        public decimal Amount { get; set; }
        [Id(2)]
        public DateTime? Date { get; set; }
        [Id(3)]
        public DateTime? DateTo { get; set; }
    }
}

[GenerateSerializer]
public class AddPaymentsReply
{
    [Id(0)]
    public List<long> PaymentIds { get; } = new();
}

[GenerateSerializer]
public class AddPaymentsForInvoicesCommand
{
    [Id(0)]
    public List<long> InvoiceIds { get; } = new();
}

[GenerateSerializer]
public sealed class Page<T>
{
    [Id(0)]
    public List<T> Items { get; } = new();
    [Id(1)]
    public int Total { get; set; }
}

[GenerateSerializer]
public sealed class AccountTypeItem
{
    [Id(0)]
    public long Id { get; set; }
    [Id(1)]
    public string Name { get; set; }
    [Id(2)]
    public string Token { get; set; }
    [Id(3)]
    public DateTime Created { get; set; }
}

[GenerateSerializer]
public sealed class GetAccountTypeQuery
{
    [Id(0)]
    public long Id { get; set; }
}

[GenerateSerializer]
public sealed class GetAccountTypesQuery
{
    [Id(0)]
    public int Offset { get; set; }
    [Id(1)]
    public int Limit { get; set; }
}

[GenerateSerializer]
public sealed class UpdateAccountTypeCommand
{
    [Id(0)]
    public string? Name { get; set; }
    [Id(1)]
    public string? Token { get; set; }
}

[GenerateSerializer]
public sealed class DeleteAccountTypeCommand
{
}

[GenerateSerializer]
public sealed class AddAccountTypeCommand
{
    [Id(0)]
    public string Name { get; set; }
    [Id(1)]
    public string Token { get; set; }
}
