namespace VoltStream.WPF.Commons.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using VoltStream.WPF.Commons.Messages;

public partial class PaymentViewModel : ViewModelBase
{
    public long Id { get; set; }
    public long CurrencyId { get; set; }
    public long CustomerId { get; set; }

    [ObservableProperty] private DateTime paidAt = DateTime.Now;
    [ObservableProperty] private decimal amount;
    [ObservableProperty] private decimal exchangeRate;
    [ObservableProperty] private decimal netAmount;
    [ObservableProperty] private decimal? discount;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private CurrencyViewModel currency = default!;
    [ObservableProperty] private CustomerViewModel customer = default!;

    // for UI
    [ObservableProperty] private decimal? incomeAmount;
    [ObservableProperty] private decimal? expenseAmount;

    [ObservableProperty] private decimal? lastBalance;
    [ObservableProperty] private decimal? balance;

    // UI control properties
    [ObservableProperty] private bool isIncomeEnabled = true;
    [ObservableProperty] private bool isExpenseEnabled = true;

    #region Property Changes

    partial void OnNetAmountChanged(decimal value)
    {
        Amount = value * ExchangeRate;
    }

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

    partial void OnCurrencyChanged(CurrencyViewModel value)
    {
        ExchangeRate = value.ExchangeRate;
    }

    #endregion Property Changes

    #region Private Helpers

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

    #endregion Private Helpers
}