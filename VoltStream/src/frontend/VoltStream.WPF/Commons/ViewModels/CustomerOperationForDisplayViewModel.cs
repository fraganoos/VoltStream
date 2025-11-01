namespace VoltStream.WPF.Commons.ViewModels;

public class CustomerOperationForDisplayViewModel
{
    public DateTime Date { get; set; }
    public string Customer { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string Description { get; set; } = string.Empty;
}
