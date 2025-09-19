namespace VoltStream.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Infrastructure.Persistence;
using VoltStream.Infrastructure.Persistence.Interceptors;
using VoltStream.Infrastructure.Web;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<AuditInterceptor>();
        services.AddDbContext<IAppDbContext, AppDbContext>((sp, options) =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));

        services.AddScoped<IPagingMetadataWriter, HttpPagingMetadataWriter>();
    }
}
