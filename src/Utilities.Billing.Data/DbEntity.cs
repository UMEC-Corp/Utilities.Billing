namespace Medlama.Common.Data;

public abstract class DbEntity<TKey> : ISoftDelible
{
    public TKey Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Deleted { get; set; }
}
