using Microsoft.EntityFrameworkCore;
using Utilities.Billing.Data.Entities;

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