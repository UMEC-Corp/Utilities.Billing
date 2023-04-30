using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.TestingHost;
using Utilities.Billing.Data;

namespace Utilities.Billing.Tests;
public abstract class GrainsFixtureBase<TSiloConfigurator> where TSiloConfigurator : ISiloConfigurator, new()
{
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var builder = new TestClusterBuilder();

        builder.AddSiloBuilderConfigurator<TSiloConfigurator>();

        Cluster = builder.Build();
        await Cluster.DeployAsync();
    }

    protected TestCluster Cluster { get; set; }

    protected BillingDbContext DbContext => SiloConfigurator.DbContext;

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await Cluster.StopAllSilosAsync();
    }
}