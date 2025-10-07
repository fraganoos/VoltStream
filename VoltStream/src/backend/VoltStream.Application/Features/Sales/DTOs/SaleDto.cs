namespace VoltStream.Application.Features.Sales.DTOs;

using VoltStream.Application.Features.CustomerOperations.DTOs;
using VoltStream.Application.Features.Customers.DTOs;

public record SaleDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; } // entity: Sale.Date
    public decimal RollCount { get; set; }    // entity: Sale.RollCount
    public decimal Length { get; set; } // entity: Sale.Length
    public decimal Amount { get; set; } // entity: Sale.Amount
    public decimal Discount { get; set; } // entity: Sale.Discount
    public string Description { get; set; } = string.Empty;

    public long CustomerOperationId { get; set; }
    public CustomerOperationDto CustomerOperation { get; set; } = default!;

    public long CustomerId { get; set; }
    public CustomerDto Customer { get; set; } = default!;

    public ICollection<SaleItemDto> SaleItems { get; set; } = default!;
}
