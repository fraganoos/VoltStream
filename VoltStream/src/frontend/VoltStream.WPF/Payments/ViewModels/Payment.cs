namespace VoltStream.WPF.Payments.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Payment
{
    public DateTime PaidAt { get; set; } = DateTime.Now; // to'langan sana
    public decimal Amount { get; set; } // orginal kirim summa
    public decimal ExchangeRate { get; set; } //dollar kurs yoki pul o'tkazish foizi
    public decimal NetAmount { get; set; } // qarz summadan ochiraldigan summa
    public string Description { get; set; } = string.Empty;
    public long CurrencyId { get; set; }
    public long CustomerId { get; set; }
}
