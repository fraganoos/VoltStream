namespace VoltStream.Application.Features.CustomerOperations.DTOs;

public record CustomerOperationSummaryDto
{
    public decimal BeginBalance { get; set; }
    public decimal EndBalance { get; set; }
    public IReadOnlyCollection<CustomerOperationDto> Operations { get; set; } = new List<CustomerOperationDto>();
}
