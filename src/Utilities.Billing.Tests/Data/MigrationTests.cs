using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Utilities.Billing.Data;

namespace Utilities.Billing.Tests.Data;

/// <remarks>
/// In memory datacontext doesn't support migrations so using sqlite
/// </remarks>>
public class MigrationTests
{
    [Test]
    public async Task MigrationsShouldSucceedOnEmptyDatabase()
    {
        var options = new DbContextOptionsBuilder<BillingDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var dbContext = new BillingDbContext(options);

        await dbContext.Database.OpenConnectionAsync();

        await dbContext.Database.MigrateAsync();

        await dbContext.Database.CloseConnectionAsync();
    }

    [Test]
    public async Task DbContextMigratorShouldMigrateEmptyDb()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<BillingDbContext>(o =>
        {
            o.UseSqlite("Data Source=:memory:");
        }, ServiceLifetime.Singleton);


        var dbContext = new BillingDbContext(serviceCollection.BuildServiceProvider()
            .GetRequiredService<DbContextOptions<BillingDbContext>>());

        var migrator = new BillingDbContextMigrator(serviceCollection.BuildServiceProvider(), Mock.Of<IHost>(),
            Mock.Of<ILogger<BillingDbContextMigrator>>());

        await dbContext.Database.OpenConnectionAsync();

        var appliedMigrations =
            await migrator.Migrate(dbContext, CancellationToken.None);

        Assert.That(appliedMigrations, Is.Not.Empty);

        await dbContext.Database.CloseConnectionAsync();

    }
}
