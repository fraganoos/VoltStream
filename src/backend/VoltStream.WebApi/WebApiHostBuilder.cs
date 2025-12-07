namespace VoltStream.WebApi;

using VoltStream.WebApi.Extensions;
using VoltStream.WebApi.Models;

public static class WebApiHostBuilder
{
    public static WebApplication Build(
        string[] args,
        IConfiguration? externalConfig = null,
        Action<RequestLog>? logCallback = null)
    {
        var builder = WebApplication.CreateBuilder(args);

        if (externalConfig is not null)
            builder.Configuration.AddConfiguration(externalConfig);

        builder.Services.AddDependencies(builder.Configuration);

        var app = builder.Build();

        app.UseInfrastructure();
        app.UseOpenApiDocumentation();
        app.UseVoltStreamMiddlewares(logCallback);

        app.MapControllers();

        return app;
    }
}
