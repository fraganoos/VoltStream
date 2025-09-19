namespace VoltStream.WPF.Supplies.Models;

using System;

public class Supply
{
    public DateTime OperationDate { get; set; } = DateTime.Now;
    public string CategoryName { get; set; } = string.Empty;
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal CountRoll { get; set; }
    public decimal QuantityPerRoll { get; set; }
    public decimal TotalQuantity { get; set; }
}
