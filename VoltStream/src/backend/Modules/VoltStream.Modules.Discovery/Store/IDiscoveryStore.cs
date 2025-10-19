namespace VoltStream.Modules.Discovery.Store;

using VoltStream.Modules.Discovery.Models;

public interface IDiscoveryStore
{
    void Register(ServiceRegistration registration);
    IEnumerable<ServiceRegistration> GetNodes(string serviceId);
}
