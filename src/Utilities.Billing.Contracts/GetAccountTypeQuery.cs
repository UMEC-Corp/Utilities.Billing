namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public sealed class GetAccountTypeQuery
{
    [Id(0)]
    public long Id { get; set; }
}
