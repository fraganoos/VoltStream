namespace VoltStream.WebApi.Middlewares;

using System.Net;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.WebApi.Models;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        int statusCode;
        string message;

        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            statusCode = (int)ex.StatusCode;
            message = ex.Message;

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
    }
}
