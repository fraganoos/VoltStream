namespace VoltStream.Application.Features.CustomerOperations.DTOs;

using VoltStream.Application.Features.Accounts.DTOs;
using VoltStream.Application.Features.Payments.DTOs;
using VoltStream.Application.Features.Sales.DTOs;
using VoltStream.Domain.Enums;

public record CustomerOperationDto
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;
    public long? CustomerId { get; set; }
    public long AccountId { get; set; }
    public AccountDto Account { get; set; } = default!;
    public DateTimeOffset Date { get; set; }

    public SaleForCustomerOperationDto? Sale { get; set; }
    public PaymentForCustomerOperationDto? Payment { get; set; }
}
public record CustomerOperationForPaymentDto
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;
    public long? CustomerId { get; set; }
    public long AccountId { get; set; }
    public AccountDto Account { get; set; } = default!;
    public DateTimeOffset Date { get; set; }
    public SaleForCustomerOperationDto? Sale { get; set; }
}

public record CustomerOperationForSaleDto
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;
    public long? CustomerId { get; set; }
    public long AccountId { get; set; }
    public AccountDto Account { get; set; } = default!;
    public DateTimeOffset Date { get; set; }
    public PaymentForCustomerOperationDto? Payment { get; set; }
}