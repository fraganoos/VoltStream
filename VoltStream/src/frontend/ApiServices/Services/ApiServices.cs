namespace ApiServices.Services;

using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class ApiServices
{
    public static IServiceCollection ConfigureServices(IServiceCollection services, string baseApiUrl)
    {
        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(
                new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                })
        };

        typeof(ApiServices).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Api"))
            .ToList()
            .ForEach(apiType => services.AddSingleton(apiType, RestService.For(apiType, baseApiUrl, refitSettings)));

        return services;
    }
}
