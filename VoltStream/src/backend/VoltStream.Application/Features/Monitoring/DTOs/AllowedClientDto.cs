namespace VoltStream.Application.Features.Monitoring.DTOs;

public class AllowedClientDto
{
    public int Id { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime CreatedAt { get; set; }
}
