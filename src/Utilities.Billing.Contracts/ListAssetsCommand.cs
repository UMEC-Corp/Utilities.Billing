namespace Utilities.Billing.Contracts
{
    [GenerateSerializer]
    public class ListAssetsCommand
    {
        [Id(0)]
        public int? Offset { get; set; }
        [Id(1)]
        public int? Limit { get; set; }
    }
}