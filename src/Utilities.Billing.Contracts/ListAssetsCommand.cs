namespace Utilities.Billing.Contracts
{
    public class ListAssetsCommand
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public string TenantId { get; set; }
    }
}