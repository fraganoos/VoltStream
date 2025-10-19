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


    // for UI
    [ObservableProperty] private decimal incomeAmount;
    [ObservableProperty] private decimal expenseAmount;

    [ObservableProperty] private decimal lastBalance;
    [ObservableProperty] private decimal balance;

    partial void OnIncomeAmountChanged(decimal value) => ReCalculateIncome();
    partial void OnExpenseAmountChanged(decimal value) => ReCalculateExpense();


    private void ReCalculateIncome()
    {
        if (ExpenseAmount != 0)
        {
            IncomeAmount = 0;
            return;
        }

        NetAmount = IncomeAmount;
        Amount = NetAmount * ExchangeRate;
        LastBalance = Balance + Amount;
    }

    private void ReCalculateExpense()
    {
        if (IncomeAmount != 0)
        {
            ExpenseAmount = 0;
            return;
        }

        IncomeAmount = 0;
        NetAmount = -ExpenseAmount;
        Amount = NetAmount * ExchangeRate;
        LastBalance = Balance + Amount;
    }
}
