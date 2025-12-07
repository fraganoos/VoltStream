namespace ApiServices.Models.Responses;

public record CustomerOperationSummaryDto
{
    public decimal BeginBalance { get; set; }
    public decimal EndBalance { get; set; }
    public IReadOnlyCollection<CustomerOperationResponse> Operations { get; set; } = new List<CustomerOperationResponse>();
}
