namespace VoltStream.ServerManager.Services;

using Microsoft.AspNetCore.SignalR.Client;
using VoltStream.ServerManager.Enums;
using VoltStream.WebApi.Models;

public class SignalRClient
{
    private HubConnection? connection;

    public event Action<RequestLog>? LogReceived;
    public event Action<ServerStatus>? StatusChanged;

    public async Task ConnectAsync(string hubUrl)
    {
        connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        connection.On<RequestLog>("ReceiveLog", log => LogReceived?.Invoke(log));
        connection.On<string>("ReceiveStatus", status =>
        {
            if (Enum.TryParse(status, out ServerStatus parsed))
                StatusChanged?.Invoke(parsed);
        });

        await connection.StartAsync();
    }
}


