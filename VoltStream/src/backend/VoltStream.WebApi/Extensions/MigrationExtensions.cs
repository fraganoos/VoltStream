namespace VoltStream.WebApi.Extensions;

using Microsoft.EntityFrameworkCore;
using VoltStream.Infrastructure.Persistence;

public static class MigrationExtensions
{
    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Database.Migrate();

        return app;
    }
}
