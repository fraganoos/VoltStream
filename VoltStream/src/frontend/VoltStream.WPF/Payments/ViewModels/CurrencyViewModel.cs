namespace VoltStream.WPF.Payments.ViewModels;

public class CurrencyViewModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public bool IsDefault { get; set; }
}