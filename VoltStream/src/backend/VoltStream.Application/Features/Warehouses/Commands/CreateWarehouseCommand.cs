namespace VoltStream.Application.Features.Warehouses.Commands;

using AutoMapper;
using MediatR;
using VoltStream.Domain.Entities;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;

public record CreateWarehouseCommand(
    long ProductId,
    decimal CountRoll,
    decimal QuantityPerRoll,
    decimal TotalQuantity) : IRequest<long>;

public class CreateWarehouseCommandHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<CreateWarehouseCommand, long>
{
    public async Task<long> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = mapper.Map<Warehouse>(request);
        context.Warehouses.Add(warehouse);
        return await context.SaveAsync(cancellationToken).ContinueWith(product => product.Id);
    }
}