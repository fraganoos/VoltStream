namespace VoltStream.Application.Features.Sales.DTOs;

public class SaleItemCreateDto
{
    public long Id { get; set; }
    public long SaleId { get; set; }
    public long ProductId { get; set; }
    public decimal CountRoll { get; set; }  // rulon soni
    public decimal QuantityPerRoll { get; set; }  //rulon uzunligi
    public decimal TotalQuantity { get; set; } // jami uzunlik
    public decimal Price { get; set; } // 1 metr narxi
    public decimal DiscountPersent { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalSumm { get; set; } // jami summa
}