namespace VoltStream.WPF.Commons.ViewModels;
public class DebitorCreditorItemViewModel
{
    public string Customer { get; set; } = string.Empty;
    public decimal Debitor { get; set; }
    public decimal Creditor { get; set; }
    public decimal TotalBalance { get; set; }

}
