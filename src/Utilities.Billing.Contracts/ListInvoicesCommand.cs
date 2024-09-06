namespace Utilities.Billing.Contracts;

[GenerateSerializer]
public class ListInvoicesCommand
{
    [Id(0)]
    public string CustomerAccountId { get; set; }
    [Id(1)]
    public ulong PeriodFrom { get; set; }
    [Id(2)]
    public ulong PeriodTo { get; set; }
}
