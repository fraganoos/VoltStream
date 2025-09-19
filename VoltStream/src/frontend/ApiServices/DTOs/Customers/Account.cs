namespace ApiServices.DTOs.Customers;

public class Account
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public decimal BeginningSumm { get; set; }
    public decimal CurrentSumm { get; set; }
    public decimal DiscountSumm { get; set; }
}