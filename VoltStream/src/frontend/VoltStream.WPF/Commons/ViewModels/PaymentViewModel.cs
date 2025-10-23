namespace VoltStream.WPF.Commons.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using VoltStream.WPF.Commons.Messages;

public partial class PaymentViewModel : ViewModelBase
{
    [ObservableProperty] private long id;
    [ObservableProperty] private DateTime paidAt = DateTime.Now; // to'langan sana
    [ObservableProperty] private decimal amount; // orginal kirim summa
    [ObservableProperty] private decimal exchangeRate; //dollar kurs yoki pul o'tkazish foizi
    [ObservableProperty] private decimal netAmount; // qarz summadan ochiraldigan summa
    [ObservableProperty] private decimal? discount; // chegirma
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private long currencyId;
    [ObservableProperty] private long customerId;

    // for UI
    [ObservableProperty] private decimal? incomeAmount;
    [ObservableProperty] private decimal? expenseAmount;

    [ObservableProperty] private decimal? lastBalance;
    [ObservableProperty] private decimal? balance;

    // UI control properties
    [ObservableProperty] private bool isIncomeEnabled = true;
    [ObservableProperty] private bool isExpenseEnabled = true;

    partial void OnExchangeRateChanged(decimal value)
    {
        ReCalculateIncome();
        ReCalculateExpense();
    }
    partial void OnIncomeAmountChanged(decimal? value)
    {
        ReCalculateIncome();
        if (value is null || value == 0)
        {
            IsExpenseEnabled = true;
            WeakReferenceMessenger.Default.Send(new FocusRequestMessage("Expense"));
        }
        else
        {
            IsExpenseEnabled = false;
            WeakReferenceMessenger.Default.Send(new FocusRequestMessage("Discription"));
        }
    }
    partial void OnExpenseAmountChanged(decimal? value)
    {
        ReCalculateExpense();
        if (value is null || value == 0)
        {
            IsIncomeEnabled = true;
            WeakReferenceMessenger.Default.Send(new FocusRequestMessage("Income"));
        }
        else
        {
            IsIncomeEnabled = false;
        }
    }


    public void ReCalculateIncome()
    {
        if (IncomeAmount.HasValue && IncomeAmount != 0)
        {
            NetAmount = (decimal)IncomeAmount;
            Amount = NetAmount * ExchangeRate;
            LastBalance = Balance + Amount;
        }
        else
        {
            NetAmount = 0;
            Amount = 0;
            LastBalance = Balance;
        }
    }

    public void ReCalculateExpense()
    {
        if (ExpenseAmount.HasValue && ExpenseAmount != 0)
        {
            NetAmount = (decimal)-ExpenseAmount;
            Amount = NetAmount * ExchangeRate;
            LastBalance = Balance + Amount;
        }
        else
        {
            NetAmount = 0;
            Amount = 0;
            LastBalance = Balance;
        }
    }
}