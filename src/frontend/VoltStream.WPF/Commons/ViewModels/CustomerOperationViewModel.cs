namespace VoltStream.WPF.Commons.ViewModels;

using ApiServices.Enums;
using ApiServices.Models.Responses;

public class CustomerOperationViewModel
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;

    public long AccountId { get; set; }
    public AccountResponse Account { get; set; } = default!;
}