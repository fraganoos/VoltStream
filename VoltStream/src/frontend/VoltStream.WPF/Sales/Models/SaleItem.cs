using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoltStream.WPF.Sales.Models
{
    public class SaleItem
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
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
    }
}
