namespace VoltStream.WPF.Supplies.Models;
using System;

public class Supply
{
    public DateTimeOffset OperationDate { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal CountRoll { get; set; }
    public decimal QuantityPerRoll { get; set; }
    public decimal TotalQuantity { get; set; }
}
