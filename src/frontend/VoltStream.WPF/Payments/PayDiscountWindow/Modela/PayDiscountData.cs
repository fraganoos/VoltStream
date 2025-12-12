namespace VoltStream.WPF.Payments.PayDiscountWindow.Modela;

public record PayDiscountData(long CustomerId, string CustomerName, decimal Discount, DateTime PaidAt);
