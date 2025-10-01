namespace VoltStream.WebApi;

using VoltStream.WebApi.Middlewares;

public static class WebApiHostBuilder
{
    public static WebApplication Build(string[] args, IConfiguration? externalConfig = null)
    {
        var builder = WebApplication.CreateBuilder(args);

        if (externalConfig is not null)
        {
            builder.Configuration.AddConfiguration(externalConfig);
        }

        // Service registrations
        builder.Services.AddDependencies(builder.Configuration);

        var app = builder.Build();

        // Middleware pipeline
        app.UseInfrastructure();
        app.UseOpenApiDocumentation();

        // ❗ faqat WPF orqali run qilinganda IP filter ishlasin
        if (externalConfig is not null)
        {
            app.UseMiddleware<ClientIpMiddleware>();
        }

        app.MapControllers();

        return app;
    }
}
