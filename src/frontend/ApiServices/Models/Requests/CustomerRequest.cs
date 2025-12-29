namespace ApiServices.Models.Requests;

public record CustomerRequest
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ClientType { get; set; }
    public List<AccountRequest> Accounts { get; set; } = default!;
}