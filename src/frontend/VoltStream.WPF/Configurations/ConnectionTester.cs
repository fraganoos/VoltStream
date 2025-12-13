namespace VoltStream.WPF.Configurations;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class ConnectionTester(IServiceProvider services)
{
    public async Task<bool> TestAsync(Action<bool>? setLoading = null)
    {
        var client = services.GetRequiredService<IHealthCheckApi>();
        var response = await client.CheckAsync().Handle(setLoading);
        return response.IsSuccess;
    }
}
