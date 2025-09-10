using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoltStream.WPF.Sales.Models
{
    public class Sale
    {
        public DateTimeOffset OperationDate { get; set; }
        public decimal CountRoll { get; set; }    // jami rulonlar soni
        public decimal TotalQuantity { get; set; } // jami metr
        public decimal Summa { get; set; } // jami summa
        public decimal Discount { get; set; } // chegirma
        public string Description { get; set; } = string.Empty;

        public long CustomerId { get; set; }
        public long CustomerOperationId { get; set; }
        public long DiscountOperationId { get; set; }
    }
}
