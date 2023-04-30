using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Billing.Data.Entities;
public class ExchangeRate : DbEntity<long>
{
    public long AccountTypeId { get; set; }
    public virtual AccountType AccountType { get; set; }
    public DateTime Effective { get; set; }
    public DateTime? Expires { get; set; }
    public decimal SellPrice { get; set; }
    public static ExchangeRate Empty { get; } = new();
}
