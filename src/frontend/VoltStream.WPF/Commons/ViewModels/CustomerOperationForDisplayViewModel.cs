namespace VoltStream.WPF.Turnovers.Models;

using ApiServices.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using VoltStream.WPF.Commons.ViewModels;
using VoltStream.WPF.Sales.ViewModels;

public partial class CustomerOperationForDisplayViewModel : ObservableObject
{
    public long Id { get; set; }
    public long? CustomerId { get; set; }
    public long AccountId { get; set; }
    [ObservableProperty] private decimal amount;
    [ObservableProperty] private DateTime date;
    [ObservableProperty] private string customer = string.Empty;
    [ObservableProperty] private decimal debit;
    [ObservableProperty] private decimal credit;
    [ObservableProperty] private string? description;
    [ObservableProperty] private OperationType operationType;
    [ObservableProperty] private AccountViewModel account = new();

    [ObservableProperty] private Sale sale;
    [ObservableProperty] private PaymentViewModel payment;

    public string FormattedDescription
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Description))
                return string.Empty;

            return string.Join("\n",
                Description.Split(';')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x)));
        }
    }
}