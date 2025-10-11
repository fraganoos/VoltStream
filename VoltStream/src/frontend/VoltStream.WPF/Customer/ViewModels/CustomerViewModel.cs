namespace VoltStream.WPF.Sales.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Customer.ViewModels;

public partial class CustomerViewModel : ViewModelBase
{
    [ObservableProperty] private long id;
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string? phone;
    [ObservableProperty] private string? address;
    [ObservableProperty] private string? description;

    [ObservableProperty] private ObservableCollection<CustomerOperationViewModel> customerOperations = [];
    [ObservableProperty] private ObservableCollection<DiscountOperationViewModel> discountOperations = [];
    private ObservableCollection<AccountViewModel> accounts = [];
    public ObservableCollection<AccountViewModel> Accounts
    {
        get => accounts;
        set
        {
            if (accounts != null)
            {
                UnsubscribeAccountEvents(accounts);
            }

            accounts = value ?? [];
            SubscribeAccountEvents(accounts);
            RecalculateBalance();
        }
    }

    public CustomerViewModel()
    {
        Accounts.CollectionChanged += Accounts_CollectionChanged;
    }

    [ObservableProperty] private decimal balance;
    [ObservableProperty] private decimal openingBalance;

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

    private void SubscribeAccountEvents(ObservableCollection<AccountViewModel> list)
    {
        foreach (var acc in list)
            acc.PropertyChanged += Account_PropertyChanged;
    }

    private void UnsubscribeAccountEvents(ObservableCollection<AccountViewModel> list)
    {
        foreach (var acc in list)
            acc.PropertyChanged -= Account_PropertyChanged;
    }

    private void RecalculateBalance()
    {
        Balance = Accounts.Sum(a => a.Balance * a.Currency.ExchangeRate);
        OpeningBalance = Accounts.Sum(a => a.OpeningBalance * a.Currency.ExchangeRate);
    }
}
