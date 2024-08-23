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
}



[GenerateSerializer]
public class UpdateAssetCommand
{
    public string Id { get; set; }
    public ICollection<string> ModelCodes { get; set; } = new List<string>();
}

[GenerateSerializer]
public class GetAssetReply
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string IssuerAccount { get; set; }
    public string MasterAccount { get; set; }
    public ICollection<string> ModelCodes { get; set; } = new List<string>();

}

[GenerateSerializer]
public class GetAssetCommand
{
    public string Id { get; set; }
}

[GenerateSerializer]
public class AddAssetReply
{
    public Guid Id { get; set; }
}

[GenerateSerializer]
public class AddAssetCommand
{
    public string AssetCode { get; set; }
    public string Issuer { get; set; }
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
