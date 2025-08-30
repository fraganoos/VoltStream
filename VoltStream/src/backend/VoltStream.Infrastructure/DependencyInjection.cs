namespace VoltStream.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltStream.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using VoltStream.Application.Commons.Interfaces;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IAppDbContext, AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}
