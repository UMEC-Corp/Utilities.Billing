
using Utilities.Billing.Data.Entities;

namespace Utilities.Billing.Contracts
{
    public class AddTenantCommand
    {
        public string Name { get; set; }
        public string Account { get; set; }
        public WalletType WalletType { get; set; }
    }
}