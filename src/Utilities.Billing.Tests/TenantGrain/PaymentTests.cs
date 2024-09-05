//using Microsoft.EntityFrameworkCore.ChangeTracking;
//using Utilities.Billing.Contracts;
//using Utilities.Billing.Data.Entities;

//namespace Utilities.Billing.Tests.TenantGrain;

//public class PaymentTests : GrainsFixtureBase<PaymentsSiloConfigurator>
//{
//    private Guid _tenantId;
//    private ITenantGrain _tenant;
//    private EntityEntry<Account> _account;
//    private EntityEntry<AccountType> _accountType;

//    [SetUp]
//    public async Task SetUp()
//    {
//        _tenantId = Guid.NewGuid();

//        _tenant = Cluster.GrainFactory.GetGrain<ITenantGrain>(_tenantId);

//        await SiloConfigurator.DbContext.Tenants.AddAsync(new Tenant
//        {
//            Id = _tenantId,
//            Currency = Guid.NewGuid().ToString(),
//            Wallet = Guid.NewGuid().ToString(),
//            Name = $"tenant-{_tenantId}"
//        });

//        _accountType = await SiloConfigurator.DbContext.AccountTypes.AddAsync(new AccountType
//        {
//            Token = Guid.NewGuid().ToString(),
//            Name = Guid.NewGuid().ToString(),
//            TenantId = _tenantId,
//            Wallet = Guid.NewGuid().ToString(),
//        });

//        var accountHolder = await SiloConfigurator.DbContext.AccountHolder.AddAsync(new AccountHolder
//        {
//            Wallet = Guid.NewGuid().ToString(),
//            TenantId = _tenantId,
//        });

//        _account = await SiloConfigurator.DbContext.Accounts.AddAsync(new Account
//        {
//            AccountHolder = accountHolder.Entity,
//            AccountType = _accountType.Entity,
//            Wallet = Guid.NewGuid().ToString(),
//        });

//        await SiloConfigurator.DbContext.SaveChangesAsync();
//    }

//    [Test]
//    public async Task Tenant_Should_Create_One_Payment_For_Single_Invoice()
//    {
//        var amount = 100m;

//        SiloConfigurator.DbContext.ExchangeRates.AddAsync(new ExchangeRate
//        {
//            AccountType = _accountType.Entity,
//            Effective = DateTime.UtcNow.AddDays(-1),
//            SellPrice = 0.5m,
//        });

//        var invoice = await SiloConfigurator.DbContext.Invoices.AddAsync(new Invoice
//        {
//            Account = _account.Entity,
//            AmountTotal = amount,
//            AmountPayed = amount / 3,
//            Date = DateTime.Now,
//        });

//        await SiloConfigurator.DbContext.SaveChangesAsync();

//        var command = new AddPaymentsForInvoicesCommand
//        {
//            InvoiceIds = { invoice.Entity.Id }
//        };

//        var addPaymentsReply = await _tenant.AddPaymentsForInvoicesAsync(command);

//        var payment = await SiloConfigurator.DbContext.Payments.FindAsync(addPaymentsReply.PaymentIds.Single());

//        Assert.That(payment, Is.Not.Null);
//        Assert.That(payment.TokenAmount, Is.EqualTo(2m * amount / 3m));
//        Assert.That(payment.CurrencyAmount, Is.EqualTo(2m * amount / 3m * 0.5m));
//        Assert.That(payment.AccountId, Is.EqualTo(_account.Entity.Id));
//    }
//}

//public class PaymentsSiloConfigurator : SiloConfigurator
//{

//}