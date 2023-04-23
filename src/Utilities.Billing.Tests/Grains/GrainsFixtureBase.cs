using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orleans.TestingHost;
using Utilities.Billing.Data;

namespace Utilities.Billing.Tests.Grains;
public abstract class GrainsFixtureBase
{
    [OneTimeSetUp]
    public async Task Setup()
    {
        var builder = new TestClusterBuilder();

        builder.AddSiloBuilderConfigurator<SiloConfigurator>();
        
        Cluster = builder.Build();
        await Cluster.DeployAsync();
    }

    protected TestCluster Cluster { get; set; }

    protected BillingDbContext DbContext => SiloConfigurator.DbContext;

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await Cluster.StopAllSilosAsync();
    }
}

class SiloConfigurator : ISiloConfigurator
{
    static SiloConfigurator()
    {
        var options = new DbContextOptionsBuilder<BillingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());

        DbContext = new BillingDbContext(options.Options);
    }

    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.ConfigureServices(serviceCollection =>
        {
            // We create a single instance of DbContext, so each test should use different tenant id to avoid conflicts.
            serviceCollection.AddSingleton(DbContext);
        });
    }

    public static BillingDbContext DbContext { get; private set; }
}
