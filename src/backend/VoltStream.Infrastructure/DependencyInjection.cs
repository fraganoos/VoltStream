namespace VoltStream.Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Infrastructure.Persistence;
using VoltStream.Infrastructure.Persistence.Interceptors;
using VoltStream.Infrastructure.Web;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration conf)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<AuditInterceptor>();
        services.AddScoped<ExcelDataSeeder>();

        services.AddDbContext<IAppDbContext, AppDbContext>((sp, opt) =>
            opt.UseNpgsql(conf.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("VoltStream.Infrastructure"))
               .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));

        services.AddScoped<IPagingMetadataWriter, HttpPagingMetadataWriter>();
    }

    public static async Task UseInfrastructureDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        var seeder = services.GetRequiredService<ExcelDataSeeder>();
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData");
        await seeder.SeedAsync(path);
    }
}
