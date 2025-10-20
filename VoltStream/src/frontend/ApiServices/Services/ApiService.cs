namespace ApiServices.Services;

using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class ApiService
{
    public static IServiceCollection ConfigureServices(IServiceCollection services)
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

        services.AddSingleton<ApiUrlHolder>();

        typeof(ApiService).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Api"))
            .ToList()
            .ForEach(apiType =>
            {
                services.AddRefitClient(apiType, refitSettings)
                    .ConfigureHttpClient((provider, client) =>
                    {
                        var urlHolder = provider.GetRequiredService<ApiUrlHolder>();
                        client.BaseAddress = new Uri(urlHolder.Url);
                    });
            });

        return services;
    }
}

public class ApiUrlHolder
{
    public string Url { get; set; } = "https://localhost:7288/api";
}