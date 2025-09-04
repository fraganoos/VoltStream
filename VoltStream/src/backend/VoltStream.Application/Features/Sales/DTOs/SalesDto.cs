namespace VoltStream.Application.Features.Sales.DTOs;

using VoltStream.Application.Features.CustomerOperations.DTOs;
using VoltStream.Application.Features.Customers.DTOs;

public class SalesDto
{
    long Id { get; set; }
    public DateTimeOffset OperationDate { get; set; } // operation kuni
    public decimal CountRoll { get; set; }    // jami rulonlar soni
    public decimal TotalQuantity { get; set; } // jami metr
    public decimal Summa { get; set; } // jami summa
    public decimal? Discount { get; set; } // chegirma
    public string Description { get; set; } = string.Empty;

    public long CustomerOperationId { get; set; }
    public CustomerOperationDto CustomerOperation { get; set; } = default!;

    public long CustomerId { get; set; }
    public CustomerDto Customer { get; set; } = default!;

    public ICollection<SaleItemDto> SaleItems { get; set; } = default!;
}