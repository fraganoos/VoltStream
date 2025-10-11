namespace VoltStream.WPF.Sales.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using VoltStream.WPF.Commons;

public partial class SaleItem : ViewModelBase
{
    public long Id { get; set; }
    [ObservableProperty] private long categoryId;
    [ObservableProperty] private string categoryName = string.Empty;
    [ObservableProperty] private long productId;
    [ObservableProperty] private string productName = string.Empty;
    [ObservableProperty] private decimal? perRollCount;
    [ObservableProperty] private decimal warehouseQuantity = decimal.Zero;
    [ObservableProperty] private decimal? rollCount;
    [ObservableProperty] private decimal warehouseCountRoll = decimal.Zero;
    [ObservableProperty] private decimal? quantity;
    [ObservableProperty] private decimal newQuantity = decimal.Zero;
    [ObservableProperty] private decimal? price;
    [ObservableProperty] private decimal? sum;
    [ObservableProperty] private decimal? perDiscount;
    [ObservableProperty] private decimal? discount;
    [ObservableProperty] private decimal? finalSumProduct;

    partial void OnPerRollCountChanged(decimal? value) => Recalculate();
    partial void OnRollCountChanged(decimal? value) => Recalculate();
    partial void OnQuantityChanged(decimal? value) => Recalculate();
    partial void OnPriceChanged(decimal? value) => Recalculate();
    partial void OnPerDiscountChanged(decimal? value) => Recalculate();
    partial void OnDiscountChanged(decimal? value) => Recalculate();

    private bool isUpdating = false;

    private void Recalculate()
    {
        if (isUpdating) return;
        try
        {
            isUpdating = true;

            // 1️⃣ Agar rulon soni yoki perRollCount mavjud bo‘lsa quantity hisobla
            if ((Quantity is null || Quantity == 0) && RollCount.HasValue && PerRollCount.HasValue)
            {
                Quantity = RollCount.Value * PerRollCount.Value;
                OnPropertyChanged(nameof(Quantity));
            }

            // 2️⃣ Sum hisoblash
            if (Price.HasValue && Quantity.HasValue)
            {
                Sum = Price.Value * Quantity.Value;
            }

            // 3️⃣ Discount (foiz yoki summa)
            if (PerDiscount.HasValue && PerDiscount.Value > 0 && Sum.HasValue)
            {
                Discount = Sum.Value * (PerDiscount.Value / 100);
            }
            else if (Discount.HasValue && Sum.HasValue && Sum.Value > 0)
            {
                PerDiscount = (Discount.Value / Sum.Value) * 100;
            }

            // 4️⃣ Yakuniy summa
            if (Sum.HasValue && Discount.HasValue)
                FinalSumProduct = Sum.Value - Discount.Value;
            else if (Sum.HasValue)
                FinalSumProduct = Sum.Value;
        }
        finally
        {
            isUpdating = false;
        }
    }
}
