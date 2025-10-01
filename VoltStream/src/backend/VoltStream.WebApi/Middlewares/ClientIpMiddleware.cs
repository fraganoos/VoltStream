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

        // ✅ Server o'zidan kelgan so'rovlarni tekshirib, DB ga kirmasdan o'tkazib yuborish
        if (IsLocalRequest(context))
        {
            await next(context);
            return;
        }

        // 🔒 Faqat tashqi IP lar uchun DB dan tekshir
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

        // IPv6 loopback (::1) yoki IPv4 loopback (127.0.0.1)
        if (IPAddress.IsLoopback(ipAddress)) return true;

        // Agar serverning o'z IP laridan biri bo'lsa
        var localIps = Dns.GetHostAddresses(Dns.GetHostName());
        return localIps.Contains(ipAddress);
    }
}
