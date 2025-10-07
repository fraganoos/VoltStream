namespace VoltStream.WebApi;

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;
using VoltStream.Application;
using VoltStream.Infrastructure;
using VoltStream.WebApi.Conventions;
using VoltStream.WebApi.Extensions;

public static class DependencyInjection
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration conf)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(conf);
        services.AddControllers(options
            => options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())))
                .AddApplicationPart(typeof(DependencyInjection).Assembly)
                .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

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

        app.ApplyMigrations();
    }
}
