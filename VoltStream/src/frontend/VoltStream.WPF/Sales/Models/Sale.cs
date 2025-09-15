namespace VoltStream.WPF.Sales.Models
{
    public class Sale
    {
        public DateTime OperationDate { get; set; } = DateTime.Now;
        public string CustomerName { get; set; } = string.Empty;
        public string CurrencyType { get; set; } = string.Empty;
        public decimal TotalSum { get; set; } // jami summa
        public decimal TotalDiscount { get; set; } // chegirma
        public decimal CountRoll { get; set; }    // jami rulonlar soni
        public decimal TotalQuantity { get; set; } // jami metr
        public string Description { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty ;
        public long CustomerId { get; set; }
        public long CustomerOperationId { get; set; }
        public long DiscountOperationId { get; set; }
    }
}
