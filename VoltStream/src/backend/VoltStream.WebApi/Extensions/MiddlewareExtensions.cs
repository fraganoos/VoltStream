namespace VoltStream.WebApi.Extensions;

using VoltStream.WebApi.Middlewares;
using VoltStream.WebApi.Models;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseVoltStreamMiddlewares(
        this IApplicationBuilder app,
        Action<RequestLog>? logCallback = null)
    {
        app.UseWhen(
            ctx => ctx.Request.Path.StartsWithSegments("/api"),
            builder => builder.UseMiddleware<ClientIpMiddleware>());

        app.UseWhen(
            ctx => ctx.Request.Path.StartsWithSegments("/api"),
            builder => builder.UseMiddleware<RequestLoggerMiddleware>(logCallback ?? (_ => { })));

        return app;
    }
}