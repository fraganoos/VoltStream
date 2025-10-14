namespace ApiServices.Models.Responses;

public record SaleResponse
{
    public long Id { get; set; }
    public DateTimeOffset Date { get; set; } // operation kuni
    public int RollCount { get; set; } // jami rulonlar soni
    public decimal Length { get; set; } // butun savdo bo'yicha jami uzunlik
    public decimal Amount { get; set; } // jami narxi
    public decimal Discount { get; set; } // chegirma narxi
    public string Description { get; set; } = string.Empty;

    public CustomerOperationResponse CustomerOperation { get; set; } = default!;
    public DiscountOperationResponse DiscountOperation { get; set; } = default!;
    public CurrencyResponse Currency { get; set; } = default!;
    public CustomerResponse Customer { get; set; } = default!;
    public ICollection<SaleItemResponse> Items { get; set; } = default!;

}
