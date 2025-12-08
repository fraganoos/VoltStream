namespace VoltStream.Application.Features.Sales.DTOs;

using VoltStream.Application.Features.Currencies.DTOs;
using VoltStream.Application.Features.Customers.DTOs;
using VoltStream.Application.Features.DiscountOperations.DTOs;

public record SaleForCustomerOperationDto
{
    public long Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public int RollCount { get; set; }
    public decimal Length { get; set; }
    public decimal Amount { get; set; }
    public decimal Discount { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsDiscountApplied { get; set; }

    public long DiscountOperationId { get; set; }
    public DiscountOperationForSaleDto DiscountOperation { get; set; } = default!;

    public long CurrencyId { get; set; }
    public CurrencyDto Currency { get; set; } = default!;

    public long CustomerId { get; set; }
    public CustomerForOperationDto Customer { get; set; } = default!;

    public ICollection<SaleItemDto> Items { get; set; } = default!;
}
