namespace VoltStream.WebApi;

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;
using VoltStream.Application;
using VoltStream.Infrastructure;
using VoltStream.WebApi.Conventions;
using VoltStream.WebApi.Extensions;
using VoltStream.WebApi.Utils;

public static class DependencyInjection
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration conf)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(conf);

        // UDP-based discovery service
        services.AddHostedService<SimpleDiscoveryResponder>();

        services.AddControllers(options =>
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())))
            .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .ConfigureApplicationPartManager(apm =>
            {
                apm.ApplicationParts.Add(new AssemblyPart(typeof(WebApiHostBuilder).Assembly));
            });

        services.AddOpenApi();
    }

    public static void UseOpenApiDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(opt =>
            {
                opt.WithTheme(ScalarTheme.BluePlanet);
                opt.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                opt.WithTitle("VoltStream API Documentation");
                opt.WithFavicon("favicon.ico");
            });
        }
    }

    public static void UseInfrastructure(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseCors(s => s
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
            app.ApplyMigrations();
    }
}
