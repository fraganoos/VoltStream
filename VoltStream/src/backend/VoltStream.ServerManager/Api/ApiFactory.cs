namespace VoltStream.ServerManager.Api;

using Refit;

public static class ApiFactory
{
    public static IAllowedClientsApi CreateAllowedClients(string baseUrl)
        => RestService.For<IAllowedClientsApi>(baseUrl);
}