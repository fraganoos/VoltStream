namespace VoltStream.Application.Features.DiscountOperations.DTOs;

using VoltStream.Application.Features.Payments.DTOs;
using VoltStream.Application.Features.Sales.DTOs;

public record DiscountOperationDto
{
    public long Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsApplied { get; set; }
    public decimal Amount { get; set; }
    public long CustomerId { get; set; }
    public long SaleId { get; set; }

    public SaleForDiscountOperationDto? Sale { get; set; }
    public PaymentForDiscountOperationDto? Payment { get; set; }
};

public record DiscountOperationForSaleDto
{
    public long Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsApplied { get; set; }
    public decimal Amount { get; set; }
    public long CustomerId { get; set; }
    public long SaleId { get; set; }
    public PaymentForDiscountOperationDto? Payment { get; set; }
};

public record DiscountOperationForPaymentDto
{
    public long Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsApplied { get; set; }
    public decimal Amount { get; set; }
    public long CustomerId { get; set; }
    public long SaleId { get; set; }
    public SaleForDiscountOperationDto? Sale { get; set; }
};

