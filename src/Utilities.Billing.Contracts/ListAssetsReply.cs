
namespace Utilities.Billing.Contracts
{

    [GenerateSerializer]
    public class AssetItem
    {
        [Id(0)]
        public Guid Id { get; set; }
        [Id(1)]
        public string Code { get; set; }
        [Id(2)]
        public string Issuer { get; set; }
    }

}