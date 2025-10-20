namespace ApiServices.Services;

using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class ApiService
{
    public static IServiceCollection ConfigureServices(IServiceCollection services, string baseApiUrl)
    {
        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter() }
                })
        };

        typeof(ApiService).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Api"))
            .ToList()
            .ForEach(apiType => services.AddSingleton(apiType, RestService.For(apiType, baseApiUrl, refitSettings)));

        return services;
    }
    public static void Reconfigure(IServiceProvider provider, string baseApiUrl)
    {
        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter() }
                })
        };

        typeof(ApiService).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Api"))
            .ToList()
            .ForEach(apiType =>
            {
                var instance = RestService.For(apiType, baseApiUrl, refitSettings);
                var field = provider.GetType().GetField("services", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field?.GetValue(provider) is IServiceCollection services)
                {
                    services.AddSingleton(apiType, instance);
                }
            });
    }
}
