namespace ApiServices.Models.Responses;

using ApiServices.Enums;

public class CustomerOperationResponse
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;

    public long AccountId { get; set; }
    public AccountResponse Account { get; set; } = default!;
    public DateTimeOffset Date { get; set; }
}
