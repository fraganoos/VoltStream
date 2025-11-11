namespace VoltStream.WebApi.Middlewares;

using System.Net;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.WebApi.Models;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            context.Response.StatusCode = (int)ex.StatusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new Response
            {
                StatusCode = (int)ex.StatusCode,
                Message = ex.Message
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new Response
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = ex.Message
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
