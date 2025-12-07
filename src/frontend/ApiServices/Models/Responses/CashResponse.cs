namespace ApiServices.Models.Responses;

public record CashResponse
{
    public long Id { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; } = true;

    public CurrencyResponse Currency { get; set; } = default!;

    public ICollection<CashOperationResponse> CashOperations { get; set; } = default!;
}