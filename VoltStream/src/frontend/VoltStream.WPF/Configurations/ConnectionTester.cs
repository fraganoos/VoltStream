namespace VoltStream.WPF.Configurations;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class ConnectionTester(IServiceProvider services)
{
    public async Task<bool> TestAsync()
    {
        var client = services.GetRequiredService<IHealthCheckApi>();
        var response = await client.GetAsync().Handle();
        return response.IsSuccess;
    }
}
