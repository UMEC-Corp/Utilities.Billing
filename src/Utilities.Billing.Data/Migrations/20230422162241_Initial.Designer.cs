﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Utilities.Billing.Data;

#nullable disable

namespace Utilities.Billing.Data.Migrations
{
    [DbContext(typeof(BillingDbContext))]
    [Migration("20230422162241_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Utilities.Billing.Data.Account", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AccountHolderId")
                        .HasColumnType("bigint")
                        .HasColumnName("account_holder_id");

                    b.Property<long>("AccountTypeId")
                        .HasColumnType("bigint")
                        .HasColumnName("account_type_id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted");

                    b.HasKey("Id")
                        .HasName("pk_accounts");

                    b.HasIndex("AccountHolderId")
                        .HasDatabaseName("ix_accounts_account_holder_id");

                    b.HasIndex("AccountTypeId")
                        .HasDatabaseName("ix_accounts_account_type_id");

                    b.ToTable("accounts", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.AccountHolder", b =>
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

            modelBuilder.Entity("Utilities.Billing.Data.AccountType", b =>
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

                    b.HasKey("Id")
                        .HasName("pk_account_types");

                    b.HasIndex("TenantId")
                        .HasDatabaseName("ix_account_types_tenant_id");

                    b.ToTable("account_types", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Invoice", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AccountId")
                        .HasColumnType("bigint")
                        .HasColumnName("account_id");

                    b.Property<decimal>("AmountPayed")
                        .HasColumnType("numeric")
                        .HasColumnName("amount_payed");

                    b.Property<decimal>("AmountTotal")
                        .HasColumnType("numeric")
                        .HasColumnName("amount_total");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted");

                    b.HasKey("Id")
                        .HasName("pk_invoices");

                    b.HasIndex("AccountId")
                        .HasDatabaseName("ix_invoices_account_id");

                    b.ToTable("invoices", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Payment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AccountId")
                        .HasColumnType("bigint")
                        .HasColumnName("account_id");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric")
                        .HasColumnName("amount");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

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

                    b.ToTable("payments", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Tenant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("currency");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_tenants");

                    b.ToTable("tenants", (string)null);
                });

            modelBuilder.Entity("Utilities.Billing.Data.Account", b =>
                {
                    b.HasOne("Utilities.Billing.Data.AccountHolder", "AccountHolder")
                        .WithMany("Accounts")
                        .HasForeignKey("AccountHolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_accounts_account_holder_account_holder_id");

                    b.HasOne("Utilities.Billing.Data.AccountType", "AccountType")
                        .WithMany()
                        .HasForeignKey("AccountTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_accounts_account_types_account_type_id");

                    b.Navigation("AccountHolder");

                    b.Navigation("AccountType");
                });

            modelBuilder.Entity("Utilities.Billing.Data.AccountHolder", b =>
                {
                    b.HasOne("Utilities.Billing.Data.Tenant", "Tenant")
                        .WithMany("AccountHolders")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_account_holder_tenants_tenant_id");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Utilities.Billing.Data.AccountType", b =>
                {
                    b.HasOne("Utilities.Billing.Data.Tenant", "Tenant")
                        .WithMany("AccountTypes")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_account_types_tenants_tenant_id");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Invoice", b =>
                {
                    b.HasOne("Utilities.Billing.Data.Account", "Account")
                        .WithMany("Invoices")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_invoices_accounts_account_id");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Payment", b =>
                {
                    b.HasOne("Utilities.Billing.Data.Account", "Account")
                        .WithMany("Payments")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_payments_accounts_account_id");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Account", b =>
                {
                    b.Navigation("Invoices");

                    b.Navigation("Payments");
                });

            modelBuilder.Entity("Utilities.Billing.Data.AccountHolder", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("Utilities.Billing.Data.Tenant", b =>
                {
                    b.Navigation("AccountHolders");

                    b.Navigation("AccountTypes");
                });
#pragma warning restore 612, 618
        }
    }
}
