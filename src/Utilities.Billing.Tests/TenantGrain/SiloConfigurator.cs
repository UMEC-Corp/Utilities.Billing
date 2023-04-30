using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Orleans.TestingHost;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data;

namespace Utilities.Billing.Tests;

public class SiloConfigurator : ISiloConfigurator
{
    static SiloConfigurator()
    {
        var options = new DbContextOptionsBuilder<BillingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings =>
            {
                warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning);
            });

        DbContext = new BillingDbContext(options.Options);
    }

    public virtual void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.ConfigureServices(services =>
        {
            // We create a single instance of DbContext, so each test should use different tenant id to avoid conflicts.
            services.AddSingleton(DbContext);
        });

        siloBuilder.ConfigureServices(services =>
        {
            var mock = new Mock<IPaymentSystem>();
            mock.Setup(x => x.CreateWalletAsync(It.IsAny<CreateWalletCommand>()))
                .Returns(Task.FromResult(Guid.NewGuid().ToString()));

            services.AddSingleton(mock.Object);
        });
    }

    public static BillingDbContext DbContext { get; }
}