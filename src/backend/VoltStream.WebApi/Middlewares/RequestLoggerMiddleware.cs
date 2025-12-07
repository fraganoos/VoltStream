namespace VoltStream.WebApi.Middlewares;

using System.Diagnostics;
using System.Net;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.WebApi.Models;

public class RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger, Action<RequestLog>? logCallback = null)
{
    public async Task Invoke(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        int statusCode = (int)HttpStatusCode.OK;
        string message = HttpStatusCode.OK.ToString();

        try
        {
            await next(context);
            statusCode = context.Response.StatusCode;
            message = ((HttpStatusCode)statusCode).ToString();
        }
        catch (AppException ex)
        {
            statusCode = (int)ex.StatusCode;
            message = ex.Message;

            logger.LogWarning(ex, "Handled AppException");

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new Response
            {
                StatusCode = statusCode,
                Message = message,
                Data = ex.Data
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
        catch (Exception ex)
        {
            statusCode = (int)HttpStatusCode.InternalServerError;
            message = "Internal Server Error";

            logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new Response
            {
                StatusCode = statusCode,
                Message = message,
                Data = new { error = ex.Message }
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
        finally
        {
            sw.Stop();

            var log = new RequestLog(
                IpAddress: context.Connection.RemoteIpAddress?.ToString(),
                Path: context.Request.Path,
                Method: context.Request.Method,
                UserAgent: context.Request.Headers.UserAgent.ToString(),
                TimeStamp: DateTime.UtcNow,
                StatusCode: statusCode,
                IsSuccess: statusCode is >= 200 and < 300,
                ElapsedMs: sw.ElapsedMilliseconds
            );

            logCallback?.Invoke(log);
        }
    }
}
