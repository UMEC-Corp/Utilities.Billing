namespace Utilities.Billing.Contracts;

public class ListInvoicesCommand
{
    public string CustomerAccountId { get; set; }
    public ulong PeriodFrom { get; set; }
    public ulong PeriodTo { get; set; }
    public string TenantId { get; set; }
}
