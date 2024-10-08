﻿using Microsoft.EntityFrameworkCore;
using Utilities.Billing.Data.Entities;
using Utilities.Common.Data;

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
    public DbSet<ExchangeRate> ExchangeRates { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<EquipmentModel> EquipmentModels { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(e =>
        {
        });

        modelBuilder.Entity<AccountHolder>(e =>
        {
            //e.HasOne(x => x.Tenant).WithMany(x => x.AccountHolders).HasForeignKey(x => x.TenantId);
        });

        modelBuilder.Entity<Account>(e =>
        {
            e.HasOne(x => x.Tenant).WithMany(x => x.Accounts).HasForeignKey(x => x.TenantId);
        });

        modelBuilder.Entity<AccountType>(e =>
        {
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Token).IsRequired();

            //e.HasOne(x => x.Tenant).WithMany(x => x.AccountTypes).HasForeignKey(x => x.TenantId);
        });

        modelBuilder.Entity<ExchangeRate>(e =>
        {
            e.HasOne(x => x.AccountType).WithMany(x => x.ExchangeRates).HasForeignKey(x => x.AccountTypeId);
        });

        //modelBuilder.Entity<Payment>(e =>
        //{
        //    e.HasOne(x => x.Account).WithMany(x => x.Payments).HasForeignKey(x => x.AccountId);
        //});

        modelBuilder.Entity<Invoice>(e =>
        {
            e.HasOne(x => x.Account).WithMany(x => x.Invoices).HasForeignKey(x => x.AccountId);
        });

        modelBuilder.Entity<Asset>(e =>
        {
            e.Property(x => x.Code).IsRequired();
            e.Property(x => x.Issuer).IsRequired();

            e.HasOne(x => x.Tenant).WithMany(x => x.Assets).HasForeignKey(x => x.TenantId);
        });

        modelBuilder.Entity<EquipmentModel>(e =>
        {
            e.HasOne(x => x.Asset).WithMany(x => x.EquipmentModels).HasForeignKey(x => x.AssetId);
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