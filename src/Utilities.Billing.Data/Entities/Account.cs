﻿namespace Utilities.Billing.Data.Entities;

public class Account : DbEntity<long>
{
    public long AccountHolderId { get; set; }
    public virtual AccountHolder AccountHolder { get; set; }
    public long AccountTypeId { get; set; }
    public virtual AccountType AccountType { get; set; }
    public virtual ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();
    public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
}