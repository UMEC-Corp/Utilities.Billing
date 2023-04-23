using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data;

namespace Utilities.Billing.Tests.Grains;
public class TenantTests : GrainsFixtureBase
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
    }
}
