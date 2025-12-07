namespace VoltStream.WPF.Commons.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using VoltStream.WPF.Customer.ViewModels;

public partial class CustomerViewModel : ViewModelBase
{
    public CustomerViewModel()
    {
        Accounts.CollectionChanged += Accounts_CollectionChanged;
    }

    [ObservableProperty] private long id;
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string? phone;
    [ObservableProperty] private string? address;
    [ObservableProperty] private string? description;

    [ObservableProperty] private ObservableCollection<CustomerOperationViewModel> customerOperations = [];
    [ObservableProperty] private ObservableCollection<DiscountOperationViewModel> discountOperations = [];
    [ObservableProperty] private ObservableCollection<AccountViewModel> accounts = [];

    [ObservableProperty] private decimal balance;
    [ObservableProperty] private decimal openingBalance;

    partial void OnAccountsChanged(ObservableCollection<AccountViewModel> value)
    {
        if (Accounts is not null)
            foreach (var account in Accounts)
                if (account.Currency is not null && account.Currency.Code == "UZS")
                    Balance = account.Balance;
    }


    private void Accounts_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (AccountViewModel acc in e.NewItems)
            {
                acc.PropertyChanged += Account_PropertyChanged;
            }
        }

        if (e.OldItems != null)
        {
            foreach (AccountViewModel acc in e.OldItems)
            {
                acc.PropertyChanged -= Account_PropertyChanged;
            }
        }

        RecalculateBalance();
    }

    private void Account_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AccountViewModel.Balance))
        {
            RecalculateBalance();
        }
    }

    private void RecalculateBalance()
    {
        Balance = Accounts.Sum(a => a.Balance * a.Currency.ExchangeRate);
        OpeningBalance = Accounts.Sum(a => a.OpeningBalance * a.Currency.ExchangeRate);
    }
}
