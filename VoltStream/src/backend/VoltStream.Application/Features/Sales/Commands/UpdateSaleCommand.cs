namespace VoltStream.Application.Features.Sales.Commands;

using MediatR;

public record UpdateSaleCommand(
    long Id,
    DateTimeOffset OperationDate,
    decimal? Discount) : IRequest<bool>;


//public DateTimeOffset OperationDate { get; set; } // operation kuni
//public long CustomerId { get; set; }
//public decimal CountRoll { get; set; }    // jami rulonlar soni
//public decimal TotalQuantity { get; set; } // jami metr
//public decimal Summa { get; set; } // jami summa
//public long CustomerOperationId { get; set; }
//public decimal? Discount { get; set; } // chegirma