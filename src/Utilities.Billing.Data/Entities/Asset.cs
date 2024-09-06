using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Common.Data;

namespace Utilities.Billing.Data.Entities
{
    public class Asset : DbEntityWithGuidKey
    {
        public Guid TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
        public string Code { get; set; }
        public string Issuer { get; set; }
        public virtual ICollection<EquipmentModel> EquipmentModels { get; set; } = new HashSet<EquipmentModel>();

    }
}
