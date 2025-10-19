namespace Discovery.Store;

using Discovery.Models;

public class InMemoryDiscoveryStore : IDiscoveryStore
{
    private readonly List<ServiceRegistration> _registrations = [];

    public void Register(ServiceRegistration registration)
    {
        // Agar shu host va serviceId bilan bor bo‘lsa – yangilaymiz
        var existing = _registrations.FirstOrDefault(x =>
            x.ServiceId == registration.ServiceId &&
            x.Host == registration.Host &&
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
        => _registrations
            .Where(x => x.ServiceId == serviceId)
            .OrderByDescending(x => x.RegisteredAt);
}
