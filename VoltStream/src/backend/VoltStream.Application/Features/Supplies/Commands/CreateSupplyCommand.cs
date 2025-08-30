namespace VoltStream.Application.Features.Supplies.Commands;

using MediatR;
using AutoMapper;
using VoltStream.Domain.Entities;
using VoltStream.Application.Commons.Interfaces;

public record CreateSupplyCommand(
    DateTimeOffset OperationDate,
    long ProductId,
    decimal CountRoll,
    decimal QuantityPerRoll,
    decimal TotalQuantity) :IRequest<long>;

public class CreateSupplyCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateSupplyCommand, long>
{
    public async Task<long> Handle(CreateSupplyCommand request, CancellationToken cancellationToken)
    {
        var supply = mapper.Map<Supply>(request);
        context.Supplies.Add(supply);
        return await context.SaveAsync(cancellationToken).ContinueWith(supply => supply.Id);
    }
}