namespace ApiServices.DTOs.Sales;

using ApiServices.DTOs.Customers;
using ApiServices.Enums;

public class CustomerOperationResponse
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public decimal Summa { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;

    public Customer Customer { get; set; } = default!;
}