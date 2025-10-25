namespace VoltStream.WebApi.Middlewares;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;
using VoltStream.WebApi.Utils;

public class ClientIpMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAppDbContext db)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrWhiteSpace(ip))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden: No IP detected.");
            return;
        }

        if (IpHelper.IsLocal(context.Connection.RemoteIpAddress))
        {
            await next(context);
            return;
        }

        var allowed = await db.AllowedClients.FirstOrDefaultAsync(c => c.IpAddress == ip);

        if (allowed is null)
        {
            db.AllowedClients.Add(new AllowedClient
            {
                IpAddress = ip,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                LastRequestAt = DateTime.UtcNow
            });
        }
        else
        {
            allowed.LastRequestAt = DateTime.UtcNow;
        }

        await db.SaveAsync(default);

        if (allowed is not null && !allowed.IsActive)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden: IP is blocked.");
            return;
        }

        await next(context);
    }
}
