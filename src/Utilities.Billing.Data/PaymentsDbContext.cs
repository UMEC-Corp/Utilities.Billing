using Medlama.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Principal;

namespace Utilities.Billing.Data;
public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options)
    {
    }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<AccountHolder> AccountHolder { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountType> AccountTypes { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Invoice> Invoices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(e =>
        {
            e.HasKey(o => o.Id);
        });

        modelBuilder.Entity<AccountHolder>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.Id).UseIdentityColumn();

            e.HasOne(x=>x.Tenant).WithMany(x=>x.AccountHolders).HasForeignKey(x=>x.TenantId);
        });

        modelBuilder.Entity<Account>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.Id).UseIdentityColumn();
        });

        modelBuilder.Entity<AccountType>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.Id).UseIdentityColumn();

            e.Property(x=>x.Name).IsRequired();
            e.Property(x=>x.Token).IsRequired();

            e.HasOne(x => x.Tenant).WithMany(x => x.AccountTypes).HasForeignKey(x => x.TenantId);
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.Id).UseIdentityColumn();

            e.HasOne(x => x.Account).WithMany(x => x.Payments).HasForeignKey(x => x.AccountId);
        });

        modelBuilder.Entity<Invoice>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.Id).UseIdentityColumn();

            e.HasOne(x => x.Account).WithMany(x => x.Invoices).HasForeignKey(x => x.AccountId);
        });

        modelBuilder.AddSoftDeleteFilters();


        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        ChangeTracker.ModifyEntries();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.ModifyEntries();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ChangeTracker.ModifyEntries();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
}

public class Tenant : DbEntity<Guid>
{
    public string Name { get; set; }
    public string Currency { get; set; }
    public virtual ICollection<AccountHolder> AccountHolders { get; set; } = new HashSet<AccountHolder>();
    public virtual ICollection<AccountType> AccountTypes { get; set; } = new HashSet<AccountType>();
}

public class AccountType : DbEntity<long>
{
    public string Name { get; set; }
    public string Token { get; set; }
    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; }
}

public class AccountHolder : DbEntity<long>
{
    public string Wallet { get; set; }
    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; }
    public decimal Credit { get; set; }
    public virtual ICollection<Account> Accounts { get; set; } = new HashSet<Account>();
}

public class Account : DbEntity<long>
{
    public long AccountHolderId { get; set; }
    public virtual AccountHolder AccountHolder { get; set; }
    public long AccountTypeId { get; set; }
    public virtual AccountType AccountType { get; set; }
    public virtual ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();
    public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
}

public class Payment : DbEntity<long>
{
    public long AccountId { get; set; }
    public virtual Account Account { get; set; }
    public decimal Amount { get; set; }
    public DateTime? Date { get; set; }
    public PaymentStatus Status { get; set; }
    public string? Transaction { get; set; }
}

public enum PaymentStatus
{
    Initial,
    Pending,
    Completed,
    Failed,
}

public class Invoice : DbEntity<long>
{
    public long AccountId { get; set; }
    public virtual Account Account { get; set; }
    public decimal AmountTotal { get; set; }
    public decimal AmountPayed { get; set; }
    public DateTime? Date { get; set; }
}

