using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Common.Data;

namespace Utilities.Billing.Data.Entities
{
    public class EquipmentModel : DbEntityWithLongKey
    {
        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public string Code { get;  set; }

    }
}
