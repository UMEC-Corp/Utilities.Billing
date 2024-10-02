namespace Utilities.Billing.Contracts
{
    [GenerateSerializer]
    public class UpdateTenantCommand
    {
        [Id(0)]
        public string Name { get; set; }
        [Id(1)]
        public string Account { get; set; }
    }
}