using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Utilities.Billing.Data;

public class BillingDbContextMigrator : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IHost _host;
    private readonly ILogger<BillingDbContextMigrator> _logger;

    public BillingDbContextMigrator(IServiceProvider services, IHost host, ILogger<BillingDbContextMigrator> logger)
    {
        _services = services;
        _host = host;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _services.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetRequiredService<BillingDbContext>();

            await Migrate(dbContext, stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e,
                "An error occurred while migrating the database. Application will be terminated to prevent data loss...");
            await _host.StopAsync(stoppingToken);
        }
    }

    internal async Task<IEnumerable<string>> Migrate(DbContext dbContext, CancellationToken stoppingToken)
    {
        var pendingMigrations = await dbContext.Database
            .GetPendingMigrationsAsync(cancellationToken: stoppingToken);

        if (pendingMigrations.Any())
        {
            _logger.LogWarning("Applying pending migrations {migrations}. ", pendingMigrations);

            await dbContext.Database.MigrateAsync(stoppingToken);
        }

        return await dbContext.Database.GetAppliedMigrationsAsync(stoppingToken);
    }
}
