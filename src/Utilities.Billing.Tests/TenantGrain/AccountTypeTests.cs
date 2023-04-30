using Microsoft.EntityFrameworkCore;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data.Entities;

namespace Utilities.Billing.Tests.TenantGrain;
public class AccountTypeTests : GrainsFixtureBase<SiloConfigurator>
{
    [Test]
    public async Task Tenant_Should_Create_AccountType()
    {
        var now = DateTime.UtcNow;

        var tenantId = Guid.NewGuid();

        var tenant = Cluster.GrainFactory.GetGrain<ITenantGrain>(tenantId);

        var command = new AddAccountTypeCommand
        {
            Name = Guid.NewGuid().ToString(),
            Token = Guid.NewGuid().ToString(),
        };

        await tenant.AddAccountTypeAsync(command);

        var accountType = await DbContext.AccountTypes.SingleAsync();

        Assert.That(accountType.Name, Is.EqualTo(command.Name));
        Assert.That(accountType.Token, Is.EqualTo(command.Token));
        Assert.That(accountType.TenantId, Is.EqualTo(tenantId));
        Assert.That(accountType.Created, Is.GreaterThanOrEqualTo(now));
        Assert.That(accountType.Wallet, Is.Not.Empty);
    }
}
