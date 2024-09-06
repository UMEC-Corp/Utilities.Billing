namespace Utilities.Billing.Contracts;

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
