using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Utilities.Billing.Data;

public abstract class DbEntity<TKey> : ISoftDelible
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public TKey Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Deleted { get; set; }
}
