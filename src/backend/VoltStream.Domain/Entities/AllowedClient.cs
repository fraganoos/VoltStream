namespace VoltStream.Domain.Entities;

public class AllowedClient : Auditable
{
    public string IpAddress { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public string? NormalizedName { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset LastRequestAt { get; set; }
}