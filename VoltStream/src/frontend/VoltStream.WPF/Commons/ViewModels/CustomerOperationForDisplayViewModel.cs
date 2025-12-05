namespace VoltStream.WPF.Commons.ViewModels;

public class CustomerOperationForDisplayViewModel
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public string Customer { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string Description { get; set; } = string.Empty;
    public string FormattedDescription
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Description))
                return string.Empty;

            // Split ; bo‘yicha va bo‘sh elementlarni tashlab yuborish
            var parts = Description
                .Split(';')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x)); // bo‘shlarni olib tashlash

            return string.Join(Environment.NewLine, parts);
        }
    }

}
