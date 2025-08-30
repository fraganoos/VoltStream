namespace VoltStream.Application.Features.Supplies.Commands;

using AutoMapper;
using MediatR;
using VoltStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;

public record UpdateSupplyCommand(
    long Id,
    DateTimeOffset OperationDate,
    long ProductId,
    decimal CountRoll,
    decimal QuantityPerRoll,
    decimal TotalQuantity) : IRequest<long>;

public class UpdateSupplyCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<UpdateSupplyCommand,long>
{
    public async Task<long> Handle(UpdateSupplyCommand request, CancellationToken cancellationToken)
    {
        var existSupply = await context.Supplies.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Supply),nameof(request.Id),request.Id);
        
        mapper.Map(request, existSupply);
        await context.SaveAsync(cancellationToken);
        return existSupply.Id;
    }
}