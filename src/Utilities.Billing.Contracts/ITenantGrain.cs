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
}

public class GetCustomerAccountReply
{
    public Guid Id { get; set; }
    public string Wallet { get; set; }
    public Guid AssetId { get; set; }
    public string AssetCode { get; set; }
    public string MasterAccount { get; set; }
}

public class GetCustomerAccountCommand
{
    public string CustomerAccountId { get; set; }
}

public class CreateCustomerAccountReply
{
    public Guid AccountId { get; set; }
}

public class CreateCustomerAccountCommand
{
    public string AssetId { get; set; }
    public string ControllerSerial { get; set; }
    public bool CreateMuxed { get; set; }
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
