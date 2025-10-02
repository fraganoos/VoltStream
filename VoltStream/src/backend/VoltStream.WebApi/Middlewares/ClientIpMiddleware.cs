namespace VoltStream.WebApi.Middlewares;

using Microsoft.EntityFrameworkCore;
using System.Net;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public class ClientIpMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAppDbContext db)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString();

        if (ip is null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden: No IP detected.");
            return;
        }

        if (IsLocalRequest(context))
        {
            await next(context);
            return;
        }

        var allowed = await db.AllowedClients
            .FirstOrDefaultAsync(c => c.IpAddress == ip);

        if (allowed is null)
        {
            allowed = new AllowedClient
            {
                IpAddress = ip,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                LastRequestAt = DateTime.UtcNow
            };

            db.AllowedClients.Add(allowed);
            await db.SaveAsync(default);
        }
        else
        {
            allowed.LastRequestAt = DateTime.UtcNow;
            await db.SaveAsync(default);
        }

        if (!allowed.IsActive)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden: IP is blocked.");
            return;
        }

        await next(context);
    }

    private static bool IsLocalRequest(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress;
        if (ipAddress is null) return false;

        if (IPAddress.IsLoopback(ipAddress)) return true;

        var localIps = Dns.GetHostAddresses(Dns.GetHostName());
        return localIps.Contains(ipAddress);
    }
}
