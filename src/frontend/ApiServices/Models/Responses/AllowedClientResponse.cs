namespace ApiServices.Models.Responses;

public class AllowedClientResponse
{
    public int Id { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastRequestAt { get; set; }
}
