namespace Discovery.Models;

public class ServiceRegistration
{
    public string ServiceId { get; set; } = default!;
    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
