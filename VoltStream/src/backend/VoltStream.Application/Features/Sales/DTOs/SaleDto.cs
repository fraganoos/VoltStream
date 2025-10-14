    namespace VoltStream.Application.Features.Sales.DTOs;

using VoltStream.Application.Features.Currencies.DTOs;
using VoltStream.Application.Features.CustomerOperations.DTOs;
using VoltStream.Application.Features.Customers.DTOs;
using VoltStream.Application.Features.DiscountOperations.DTOs;

public record SaleDto
{
    public long Id { get; set; }
    public DateTimeOffset Date { get; set; } // operation kuni
    public int RollCount { get; set; } // jami rulonlar soni
    public decimal Length { get; set; } // butun savdo bo'yicha jami uzunlik
    public decimal Amount { get; set; } // jami narxi
    public decimal Discount { get; set; } // chegirma narxi
    public string Description { get; set; } = string.Empty;
    public CustomerOperationDto CustomerOperation { get; set; } = default!;
    public DiscountOperationDto DiscountOperation { get; set; } = default!;
    public CurrencyDto Currency { get; set; } = default!;
    public CustomerDto Customer { get; set; } = default!;
    public ICollection<SaleItemDto> Items { get; set; } = default!;

}
