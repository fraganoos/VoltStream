namespace VoltStream.WPF.Commons.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class PaymentViewModel : ViewModelBase
{
    [ObservableProperty] private long id;
    [ObservableProperty] private DateTime paidAt = DateTime.Now; // to'langan sana
    [ObservableProperty] private decimal amount; // orginal kirim summa
    [ObservableProperty] private decimal exchangeRate; //dollar kurs yoki pul o'tkazish foizi
    [ObservableProperty] private decimal netAmount; // qarz summadan ochiraldigan summa
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private long currencyId;
    [ObservableProperty] private long customerId;
}
