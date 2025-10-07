namespace VoltStream.Domain.Entities;

public class SaleItem : Auditable
{
    public decimal RollCount { get; set; }  // rulon soni
    public decimal LengthPerRoll { get; set; }  //rulon uzunligi
    public decimal TotalLength { get; set; } // jami uzunlik
    public decimal UnitPrice { get; set; } // 1 metr narxi
    public decimal DiscountRate { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; } // jami summa

    public long SaleId { get; set; }
    public Sale Sale { get; set; } = default!;

    public long ProductId { get; set; }
    public Product Product { get; set; } = default!;
}