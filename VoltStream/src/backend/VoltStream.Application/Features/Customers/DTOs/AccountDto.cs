namespace VoltStream.Application.Features.Customers.DTOs;

public record AccountDto
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal Balance { get; set; }
    public decimal Discount { get; set; }
    public long CurrencyId { get; set; }
}

public record AccountCreationDto
{
    public decimal OpeningBalance { get; set; }
    public decimal Balance { get; set; }
    public decimal Discount { get; set; }
    public long CurrencyId { get; set; }
}