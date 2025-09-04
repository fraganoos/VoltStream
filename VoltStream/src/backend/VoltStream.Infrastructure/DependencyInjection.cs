namespace VoltStream.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Infrastructure.Persistence;
using VoltStream.Infrastructure.Persistence.Interceptors;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditInterceptor>();
        services.AddDbContext<IAppDbContext, AppDbContext>((sp, options) =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));

        return services;
    }
}
