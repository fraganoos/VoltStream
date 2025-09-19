namespace VoltStream.Application.Features.Customers.DTOs;

public record AccountDto
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public decimal BeginningSumm { get; set; }
    public decimal CurrentSumm { get; set; }
    public decimal DiscountSumm { get; set; }
}