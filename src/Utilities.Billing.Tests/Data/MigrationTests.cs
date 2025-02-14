﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utilities.Billing.Data;

namespace Utilities.Billing.Tests.Data;

/// <remarks>
/// In memory DbContext doesn't support migrations so using sqlite
/// </remarks>>
public class MigrationTests
{
    [Test]
    public async Task MigrationsShouldSucceedOnEmptyDatabase()
    {
        var services = new ServiceCollection().AddLogging(logging =>
        {
            logging.AddConsole();
        }).BuildServiceProvider();

        var options = new DbContextOptionsBuilder<BillingDbContext>()
            .UseSqlite("Data Source=:memory:", b => b.MigrationsAssembly("Utilities.Billing.Tests"))
            .UseLoggerFactory(services.GetRequiredService<ILoggerFactory>())
            .Options;

        await using var dbContext = new BillingDbContext(options);

        await dbContext.Database.OpenConnectionAsync();

        await dbContext.Database.MigrateAsync();

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync();

        Assert.That(appliedMigrations, Is.Not.Empty);
        Assert.That(pendingMigrations, Is.Empty);

        await dbContext.Database.CloseConnectionAsync();
    }

    [Test]
    public async Task DbContextMigratorShouldMigrateEmptyDb()
    {
        Assert.Inconclusive("This test should be moved to the Common repo");
        //var serviceCollection = new ServiceCollection();

        //serviceCollection.AddDbContext<BillingDbContext>(o =>
        //{
        //    o.UseSqlite("Data Source=:memory:");
        //}, ServiceLifetime.Singleton);


        //var dbContext = new BillingDbContext(serviceCollection.BuildServiceProvider()
        //    .GetRequiredService<DbContextOptions<BillingDbContext>>());

        //var migrator = new DbContextMigrator<BillingDbContext>(serviceCollection.BuildServiceProvider(), Mock.Of<IHost>(),
        //    Mock.Of<ILogger<DbContextMigrator<BillingDbContext>>>());

        //await dbContext.Database.OpenConnectionAsync();

        //var appliedMigrations =
        //    await migrator.Migrate(dbContext, CancellationToken.None);

        //Assert.That(appliedMigrations, Is.Not.Empty);

        //await dbContext.Database.CloseConnectionAsync();

    }
}
