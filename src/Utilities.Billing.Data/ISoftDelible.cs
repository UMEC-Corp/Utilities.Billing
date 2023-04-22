namespace Medlama.Common.Data;

public interface ISoftDelible
{
    DateTime Created { get; set; }
    DateTime? Deleted { get; set; }
}