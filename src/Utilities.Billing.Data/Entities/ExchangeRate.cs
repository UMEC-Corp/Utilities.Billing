using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Common.Data;

namespace Utilities.Billing.Data.Entities;
public class ExchangeRate : DbEntityWithLongKey
{
    public long AccountTypeId { get; set; }
    public virtual AccountType AccountType { get; set; }
    public DateTime Effective { get; set; }
    public DateTime? Expires { get; set; }
    public decimal SellPrice { get; set; }
}
