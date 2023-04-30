using Microsoft.EntityFrameworkCore.ChangeTracking;
using Utilities.Billing.Contracts;
using Utilities.Billing.Data.Entities;

namespace Utilities.Billing.Tests.TenantGrain;
public class InvoiceTests : GrainsFixtureBase<InvoicesSiloConfigurator>
{
    private Guid _tenantId;
    private ITenantGrain _tenant;
    private EntityEntry<Account> _account;

    [SetUp]
    public async Task SetUp()
    {
        _tenantId = Guid.NewGuid();

        _tenant = Cluster.GrainFactory.GetGrain<ITenantGrain>(_tenantId);

        var accountType = await SiloConfigurator.DbContext.AccountTypes.AddAsync(new AccountType
        {
            Token = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
            TenantId = _tenantId,
            Wallet = Guid.NewGuid().ToString(),
        });

        var accountHolder = await SiloConfigurator.DbContext.AccountHolder.AddAsync(new AccountHolder
        {
            Wallet = Guid.NewGuid().ToString(),
            TenantId = _tenantId,
        });

        _account = await SiloConfigurator.DbContext.Accounts.AddAsync(new Account
        {
            AccountHolder = accountHolder.Entity,
            AccountType = accountType.Entity,
            Wallet = Guid.NewGuid().ToString(),
        });

        await SiloConfigurator.DbContext.SaveChangesAsync();
    }

    [Test]
    public async Task Tenant_Should_Create_Single_Invoice()
    {
        var amount = 1.0m;

        await SiloConfigurator.DbContext.SaveChangesAsync();

        var command = new AddInvoicesCommand
        {
            Items = { 
                new AddInvoicesCommand.Item
                {
                    AccountId = _account.Entity.Id,
                    Amount = amount,
                }
            }
        };

        var reply = await _tenant.AddInvoicesAsync(command);

        var invoiceId = reply.InvoiceIds.Single();

        var invoice = await SiloConfigurator.DbContext.Invoices.FindAsync(invoiceId);

        Assert.That(invoice, Is.Not.Null);
        Assert.That(invoice.AccountId, Is.EqualTo(_account.Entity.Id));
        Assert.That(invoice.AmountTotal, Is.EqualTo(amount));
    }
}

public class InvoicesSiloConfigurator : SiloConfigurator
{

}
