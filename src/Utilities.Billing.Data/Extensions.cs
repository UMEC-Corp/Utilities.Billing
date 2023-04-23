using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Utilities.Billing.Data;
public static class Extensions
{
    public static ModelBuilder AddSoftDeleteFilters(this ModelBuilder modelBuilder)
    {
        var entities = modelBuilder.Model.GetEntityTypes().Select(x => x.ClrType)
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ISoftDelible)))
            .OrderBy(x => x.Name);

        foreach (var type in entities)
        {
            var x = Expression.Parameter(type);
            var filter = Expression.Lambda(Expression.Equal(Expression.Property(x, nameof(ISoftDelible.Deleted)),
                Expression.Constant(null)), x);

            modelBuilder.Entity(type, e =>
            {
                // Enables soft deletes
                e.HasQueryFilter(filter);
            });
        }

        return modelBuilder;
    }

    public static void ModifyEntries(this ChangeTracker changeTracker)
    {
        foreach (var entry in changeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                entry.CurrentValues[nameof(ISoftDelible.Created)] = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Deleted)
            {
                entry.CurrentValues[nameof(ISoftDelible.Deleted)] = DateTime.UtcNow;
                entry.State = EntityState.Modified;
            }
        }
    }
}
