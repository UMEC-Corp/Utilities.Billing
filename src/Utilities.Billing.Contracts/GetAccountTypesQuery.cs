namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public sealed class GetAccountTypesQuery
{
    [Id(0)]
    public int Offset { get; set; }
    [Id(1)]
    public int Limit { get; set; }
}
