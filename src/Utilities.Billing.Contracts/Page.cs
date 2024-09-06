namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public sealed class Page<T>
{
    [Id(0)]
    public List<T> Items { get; } = new();
    [Id(1)]
    public int Total { get; set; }
}
