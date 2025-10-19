namespace Discovery.Store;

using Discovery.Models;

public interface IDiscoveryStore
{
    void Register(ServiceRegistration registration);
    IEnumerable<ServiceRegistration> GetNodes(string serviceId);
}
