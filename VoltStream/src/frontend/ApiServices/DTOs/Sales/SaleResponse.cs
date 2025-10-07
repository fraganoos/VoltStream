namespace ApiServices.DTOs.Sales;

using ApiServices.DTOs.Customers;

public class SaleResponse
{
    long Id { get; set; }
    public DateTimeOffset OperationDate { get; set; }
    public decimal CountRoll { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal Summa { get; set; }
    public decimal? Discount { get; set; }
    public string Description { get; set; } = string.Empty;

    public long CustomerOperationId { get; set; }
    public CustomerOperationResponse CustomerOperation { get; set; } = default!;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public ICollection<SaleItemResponse> SaleItems { get; set; } = default!;
}
