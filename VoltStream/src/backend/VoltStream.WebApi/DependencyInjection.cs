namespace VoltStream.WebApi;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Scalar.AspNetCore;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using VoltStream.Application;
using VoltStream.Infrastructure;
using VoltStream.WebApi.Conventions;
using VoltStream.WebApi.Extensions;

public static class DependencyInjection
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration conf)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(conf);

        // Module Discovery conf
        //services.AddDiscoveryModule(conf);

        // local discovery
        services.AddHostedService<SimpleDiscoveryResponder>();

        services.AddControllers(options
            => options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())))
                .AddApplicationPart(typeof(RegistryController).Assembly)
                .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddOpenApi();
    }

    public static void UseOpenApiDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(opt =>
            {
                opt.WithTheme(ScalarTheme.BluePlanet);
                opt.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                opt.WithTitle("VoltStream API Documentation");
                opt.WithFavicon("favicon.ico");
            });
        }
    }

    public static void UseInfrastructure(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseCors(s => s
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseAuthorization();

        app.ApplyMigrations();
    }
}

public class SimpleDiscoveryResponder(IServer server, IHostEnvironment env, ILogger<SimpleDiscoveryResponder> logger) : BackgroundService
{
    private const int ListenPort = 5001;
    private readonly IServerAddressesFeature? _serverAddresses = server.Features.Get<IServerAddressesFeature>();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var udp = new UdpClient(ListenPort);
        logger.LogInformation("📡 Discovery responder listening on UDP port {port}", ListenPort);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = await udp.ReceiveAsync(stoppingToken);
                var msg = Encoding.UTF8.GetString(result.Buffer).Trim();

                if (msg == "DISCOVER")
                {
                    var uri = GetServerUri();
                    var ip = GetLocalIp();
                    var response = $"{uri?.Scheme ?? "http"}://{ip}:{uri?.Port ?? 5000}";

                    var bytes = Encoding.UTF8.GetBytes(response);
                    await udp.SendAsync(bytes, bytes.Length, result.RemoteEndPoint);
                    logger.LogInformation("✅ Sent discovery response: {response} to {remote}", response, result.RemoteEndPoint);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("⚠️ Discovery responder error: {msg}", ex.Message);
            }
        }
    }

    private Uri? GetServerUri()
    {
        var raw = _serverAddresses?.Addresses.FirstOrDefault();
        return Uri.TryCreate(raw, UriKind.Absolute, out var uri) ? uri : null;
    }

    private string GetLocalIp()
    {
        if (env.IsDevelopment())
            return "localhost";

        var host = Dns.GetHostEntry(Dns.GetHostName());

        // Prioritize real network IPs (Ethernet, Wi-Fi)
        var preferred = host.AddressList
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
            .Where(ip => !IPAddress.IsLoopback(ip))
            .Where(ip => ip.ToString().StartsWith("192."))
            .FirstOrDefault();

        return preferred?.ToString() ?? "localhost";
    }
}
