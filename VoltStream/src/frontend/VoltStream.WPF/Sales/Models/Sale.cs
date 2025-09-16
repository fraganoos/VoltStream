namespace VoltStream.WPF.Sales.Models
{
    public class Sale
    {
        public DateTime OperationDate { get; set; } = DateTime.Now;
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CurrencyType { get; set; } = string.Empty;
        public decimal? TotalSum { get; set; } // jami summa
        public decimal? TotalDiscount { get; set; } // chegirma
        public bool CheckedDiscount { get; set; } // chegirma ishlatilganmi
        public decimal? FinalSum { get; set; } // chegirmadan keyingi summa
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public long CategoryId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string PerRollCount { get; set; } = string.Empty; // birligi
        public decimal? RollCount { get; set; } // rulon
        public decimal? Quantity { get; set; } // metr
        public decimal? Price { get; set; } // narxi
        public decimal? Sum { get; set; } // summa
        public decimal? PerDiscount { get; set; } // chegirma
        public decimal? Discount { get; set; } // chegirma summasi
        public decimal? FinalSumProduct { get; set; } // chegirmadan keyingi narxi
        public List<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
