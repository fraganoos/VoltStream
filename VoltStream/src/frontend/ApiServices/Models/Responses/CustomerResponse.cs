namespace ApiServices.Models.Responses;
public record CustomerResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }

    public ICollection<AccountResponse> Accounts { get; set; } = default!;
    public ICollection<CustomerOperationResponse> CustomerOperations { get; set; } = default!;
    public ICollection<DiscountOperationResponse> DiscountOperations { get; set; } = default!;
}

