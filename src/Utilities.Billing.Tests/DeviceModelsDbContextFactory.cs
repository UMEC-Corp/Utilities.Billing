using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Utilities.Billing.Data;

namespace Utilities.Billing.Tests;
internal class BillingDbContextFactory : IDesignTimeDbContextFactory<BillingDbContext>
{
    public BillingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BillingDbContext>();
        optionsBuilder.UseSqlite("Data Source=:memory:", b => b.MigrationsAssembly("Utilities.Billing.Tests"));

        return new BillingDbContext(optionsBuilder.Options);
    }
}
