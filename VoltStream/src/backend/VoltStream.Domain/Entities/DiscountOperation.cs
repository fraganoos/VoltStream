namespace VoltStream.Domain.Entities;

using System;

public class DiscountOperation : Auditable
{
    public DateTimeOffset Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Summa { get; set; }
    public bool IsDiscountUsed { get; set; }
    public decimal DiscountSumm { get; set; }

    public long AccountId { get; set; }
    public Account Account { get; set; } = default!;
}