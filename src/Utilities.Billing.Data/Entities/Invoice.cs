﻿using Utilities.Common.Data;

namespace Utilities.Billing.Data.Entities;

public class Invoice : DbEntityWithLongKey
{
    public long AccountId { get; set; }
    public virtual Account Account { get; set; }
    public decimal AmountTotal { get; set; }
    public decimal AmountPayed { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? DateTo { get; set; }
}