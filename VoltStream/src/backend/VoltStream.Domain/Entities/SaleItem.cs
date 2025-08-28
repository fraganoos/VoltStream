namespace VoltStream.Domain.Entities;

public class SaleItem : Auditable
{
    public long SaleId { get; set; }
    public long ProductId { get; set; }
    public decimal CountRoll { get; set; }  // rulon soni
    public decimal QuantityPerRoll { get; set; }  //rulon uzunligi
    public decimal TotalQuantity { get; set; } // jami uzunlik
    public decimal Price { get; set; } // 1 metr narxi
    public decimal TotalSumm { get; set; } // jami summa
}