namespace VoltStream.Application.Features.Sales.Commands;

using AutoMapper;
using MediatR;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;

public record CreateSaleCommand(
    DateTimeOffset OperationDate,
    decimal? Discount,
    List<SaleItem> SaleItems) : IRequest<long>;

public class CreateSaleCommandHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<CreateSaleCommand, long>
{
    public async Task<long> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        await context.BeginTransactionAsync(cancellationToken);

        var sale = mapper.Map<Sale>(request);
        var customerOperation = mapper.Map<CustomerOperation>(request);
        customerOperation.OperationType = OperationType.Sale;
        sale.CustomerOperation = customerOperation;

        context.Sales.Add(sale);
        await context.CommitTransactionAsync(cancellationToken);

        return sale.Id;
    }
}
