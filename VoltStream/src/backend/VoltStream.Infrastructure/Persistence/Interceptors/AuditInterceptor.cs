namespace VoltStream.Infrastructure.Persistence.Interceptors;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VoltStream.Domain.Entities;

public class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        NormalizeDateTimes(eventData.Context);
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        NormalizeDateTimes(eventData.Context);
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateAuditFields(DbContext? context)
    {
        if (context is null) return;

        var entries = context.ChangeTracker.Entries<Auditable>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = now;
            else if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = now;
        }
    }

    private static void NormalizeDateTimes(DbContext? context)
    {
        if (context is null) return;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var properties = entry.Entity.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

            foreach (var prop in properties)
            {
                var value = prop.GetValue(entry.Entity);

                if (value is DateTime dt)
                {
                    if (dt.Kind == DateTimeKind.Local)
                        prop.SetValue(entry.Entity, dt.ToUniversalTime());
                    else if (dt.Kind == DateTimeKind.Unspecified)
                        prop.SetValue(entry.Entity, DateTime.SpecifyKind(dt, DateTimeKind.Utc));
                }
            }
        }
    }
}
