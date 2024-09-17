
namespace Utilities.Billing.Contracts
{
    [GenerateSerializer]
    public class AddTenantReply
    {
        [Id(0)]
        public Guid Id { get; set; }
    }
}