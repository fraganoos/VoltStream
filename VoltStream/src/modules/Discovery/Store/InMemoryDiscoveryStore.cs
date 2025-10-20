namespace Discovery.Store;

using Discovery.Models;

public class InMemoryDiscoveryStore : IDiscoveryStore
{
    private readonly List<ServiceRegistration> _registrations = [];

    public void Register(ServiceRegistration registration)
    {
        var existing = _registrations.FirstOrDefault(x =>
            x.ServiceId == registration.ServiceId &&
            x.IpAddress == registration.IpAddress &&
            x.Port == registration.Port);

        if (existing is not null)
        {
            existing.RegisteredAt = DateTime.UtcNow;
        }
        else
        {
            _registrations.Add(registration);
        }
    }

    public IEnumerable<ServiceRegistration> GetNodes(string serviceId)
    {
        var now = DateTime.UtcNow;

        return _registrations
            .Where(x => x.ServiceId == serviceId && (now - x.RegisteredAt).TotalSeconds < 60)
            .OrderByDescending(x => x.RegisteredAt);
    }
}
