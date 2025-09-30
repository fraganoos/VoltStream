namespace VoltStream.WPF.Sales.Models
{
    using System.Collections.ObjectModel;
    using VoltStream.WPF.Commons;

    public class Sale : ViewModelBase
    {
        public decimal? _totalSum;
        public decimal? _totalDiscount;
        public decimal? _finalSum;
        public DateTime OperationDate { get; set; } = DateTime.Now;
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CurrencyType { get; set; } = string.Empty;
        public decimal? TotalSum
        {
            get => _totalSum;
            set
            {
                _totalSum = value;
                OnPropertyChanged(nameof(TotalSum));
            }
        } // jami summa
        public bool CheckedDiscount { get; set; } = false; // chegirma ishlatilganmi
        public decimal? TotalDiscount
        {
            get => _totalDiscount;
            set
            {
                _totalDiscount = value;
                OnPropertyChanged(nameof(TotalDiscount));
            }
        } // chegirma
        public decimal? FinalSum
        {
            get => _finalSum;
            set
            {
                _finalSum = value;
                OnPropertyChanged(nameof(FinalSum));
            }
        }// chegirmadan keyingi summa
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public long CategoryId { get; set; }
        public long ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? PerRollCount { get; set; }  // birligi
        public string? RollCount { get; set; } // rulon
        public string? Quantity { get; set; } // metr
        public string? Price { get; set; } // narxi
        public string? Sum { get; set; } // summa
        public string? PerDiscount { get; set; } // chegirma
        public string? Discount { get; set; } // chegirma summasi
        public string? FinalSumProduct { get; set; } // chegirmadan keyingi narxi
        public ObservableCollection<SaleItem> SaleItems { get; set; } = new ObservableCollection<SaleItem>();
    }
}
