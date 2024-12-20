﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Utilities.Billing.Data;

#nullable disable

namespace Utilities.Billing.Data.Migrations
{
    [DbContext(typeof(BillingDbContext))]
    partial class BillingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Utilities.Billing.Data.Entities.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long?>("AccountHolderId")
                        .HasColumnType("bigint")
                        .HasColumnName("account_holder_id");

                    b.Property<long?>("AccountTypeId")
                        .HasColumnType("bigint")
                        .HasColumnName("account_type_id");

                    b.Property<Guid>("AssetId")
                        .HasColumnType("uuid")
                        .HasColumnName("asset_id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted");

                    b.Property<string>("DeviceSerial")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("device_serial");

                    b.Property<string>("InputCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("input_code");

                    b.Property<int>("State")
                        .HasColumnType("integer")
                        .HasColumnName("state");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.Property<string>("Wallet")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("wallet");

                    b.HasKey("Id")
                        .HasName("pk_accounts");

                    b.HasIndex("AccountHolderId")
                        .HasDatabaseName("ix_accounts_account_holder_id");

                    b.HasIndex("AccountTypeId")
                        .HasDatabaseName("ix_accounts_account_type_id");

                    b.HasIndex("AssetId")
                        .HasDatabaseName("ix_accounts_asset_id");

                    b.HasIndex("TenantId")
                        .HasDatabaseName("ix_accounts_tenant_id");

                    b.ToTable("accounts", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.AccountHolder", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<decimal>("Credit")
                        .HasColumnType("numeric")
                        .HasColumnName("credit");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.Property<string>("Wallet")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("wallet");

                    b.HasKey("Id")
                        .HasName("pk_account_holder");

                    b.HasIndex("TenantId")
                        .HasDatabaseName("ix_account_holder_tenant_id");

                    b.ToTable("account_holder", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.AccountType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("token");

                    b.Property<string>("Wallet")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("wallet");

                    b.HasKey("Id")
                        .HasName("pk_account_types");

                    b.HasIndex("TenantId")
                        .HasDatabaseName("ix_account_types_tenant_id");

                    b.ToTable("account_types", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.Asset", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted");

                    b.Property<string>("Issuer")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("issuer");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.HasKey("Id")
                        .HasName("pk_assets");

                    b.HasIndex("TenantId")
                        .HasDatabaseName("ix_assets_tenant_id");

                    b.ToTable("assets", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.EquipmentModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid>("AssetId")
                        .HasColumnType("uuid")
                        .HasColumnName("asset_id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted");

                    b.HasKey("Id")
                        .HasName("pk_equipment_models");

                    b.HasIndex("AssetId")
                        .HasDatabaseName("ix_equipment_models_asset_id");

                    b.ToTable("equipment_models", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.ExchangeRate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AccountTypeId")
                        .HasColumnType("bigint")
                        .HasColumnName("account_type_id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted");

                    b.Property<DateTime>("Effective")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("effective");

                    b.Property<DateTime?>("Expires")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expires");

                    b.Property<decimal>("SellPrice")
                        .HasColumnType("numeric")
                        .HasColumnName("sell_price");

                    b.HasKey("Id")
                        .HasName("pk_exchange_rates");

                    b.HasIndex("AccountTypeId")
                        .HasDatabaseName("ix_exchange_rates_account_type_id");

                    b.ToTable("exchange_rates", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.Invoice", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid")
                        .HasColumnName("account_id");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric")
                        .HasColumnName("amount");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted");

                    b.Property<string>("PayerWallet")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("payer_wallet");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<string>("Xdr")
                        .HasColumnType("text")
                        .HasColumnName("xdr");

                    b.HasKey("Id")
                        .HasName("pk_invoices");

                    b.HasIndex("AccountId")
                        .HasDatabaseName("ix_invoices_account_id");

                    b.ToTable("invoices", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.Payment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid")
                        .HasColumnName("account_id");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric")
                        .HasColumnName("amount");

                    b.Property<Guid>("AssetId")
                        .HasColumnType("uuid")
                        .HasColumnName("asset_id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<string>("Transaction")
                        .HasColumnType("text")
                        .HasColumnName("transaction");

                    b.HasKey("Id")
                        .HasName("pk_payments");

                    b.HasIndex("AccountId")
                        .HasDatabaseName("ix_payments_account_id");

                    b.HasIndex("AssetId")
                        .HasDatabaseName("ix_payments_asset_id");

                    b.ToTable("payments", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.Tenant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Wallet")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("wallet");

                    b.HasKey("Id")
                        .HasName("pk_tenants");

                    b.ToTable("tenants", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.Account", b =>
                {
                    b.HasOne("Utilities.Billing.Data.Entities.AccountHolder", null)
                        .WithMany("Accounts")
                        .HasForeignKey("AccountHolderId")
                        .HasConstraintName("fk_accounts_account_holder_account_holder_id");

                    b.HasOne("Utilities.Billing.Data.Entities.AccountType", null)
                        .WithMany("Accounts")
                        .HasForeignKey("AccountTypeId")
                        .HasConstraintName("fk_accounts_account_types_account_type_id");

                    b.HasOne("Utilities.Billing.Data.Entities.Asset", "Asset")
                        .WithMany()
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_accounts_assets_asset_id");

                    b.HasOne("Utilities.Billing.Data.Entities.Tenant", "Tenant")
                        .WithMany("Accounts")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_accounts_tenants_tenant_id");

                    b.Navigation("Asset");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.AccountHolder", b =>
                {
                    b.HasOne("Utilities.Billing.Data.Entities.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_account_holder_tenants_tenant_id");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.AccountType", b =>
                {
                    b.HasOne("Utilities.Billing.Data.Entities.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_account_types_tenants_tenant_id");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.Asset", b =>
                {
                    b.HasOne("Utilities.Billing.Data.Entities.Tenant", "Tenant")
                        .WithMany("Assets")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_assets_tenants_tenant_id");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.EquipmentModel", b =>
                {
                    b.HasOne("Utilities.Billing.Data.Entities.Asset", "Asset")
                        .WithMany("EquipmentModels")
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_equipment_models_assets_asset_id");

                    b.Navigation("Asset");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.ExchangeRate", b =>
                {
                    b.HasOne("Utilities.Billing.Data.Entities.AccountType", "AccountType")
                        .WithMany("ExchangeRates")
                        .HasForeignKey("AccountTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_exchange_rates_account_types_account_type_id");

                    b.Navigation("AccountType");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.Invoice", b =>
                {
                    b.HasOne("Utilities.Billing.Data.Entities.Account", "Account")
                        .WithMany("Invoices")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_invoices_accounts_account_id");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.Payment", b =>
                {
                    b.HasOne("Utilities.Billing.Data.Entities.Account", "Account")
                        .WithMany("Payments")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_payments_accounts_account_id");

                    b.HasOne("Utilities.Billing.Data.Entities.Asset", "Asset")
                        .WithMany()
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_payments_assets_asset_id");

                    b.Navigation("Account");

                    b.Navigation("Asset");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.Account", b =>
                {
                    b.Navigation("Invoices");

                    b.Navigation("Payments");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.AccountHolder", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.AccountType", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("ExchangeRates");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.Asset", b =>
                {
                    b.Navigation("EquipmentModels");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Entities.Tenant", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("Assets");
                });
#pragma warning restore 612, 618
        }
    }
}
