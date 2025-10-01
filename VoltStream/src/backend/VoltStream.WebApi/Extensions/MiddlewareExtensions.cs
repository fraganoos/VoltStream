namespace VoltStream.WebApi.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app, Action<RequestLog>? callback = null)
    {
        app.Use(async (context, next) =>
        {
            var log = new RequestLog
            {
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                Path = context.Request.Path,
                Method = context.Request.Method,
                UserAgent = context.Request.Headers.UserAgent.ToString(),
                TimeStamp = DateTime.UtcNow
            };

            callback?.Invoke(log);

            await next();
        });

        return app;
    }
}

public class RequestLog
{
    public string? IpAddress { get; set; }
    public string? Path { get; set; }
    public string? Method { get; set; }
    public string? UserAgent { get; set; }
    public DateTime TimeStamp { get; set; }
}
