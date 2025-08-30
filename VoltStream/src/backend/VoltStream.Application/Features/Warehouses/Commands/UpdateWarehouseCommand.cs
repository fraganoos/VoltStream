namespace VoltStream.Application.Features.Warehouses.Commands;

using MediatR;
using AutoMapper;
using VoltStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;

public record UpdateWarehouseCommand(long Id) :IRequest<long>;

public class UpdateWarehouseCommandHandler(
    IAppDbContext context,
    IMapper mapper) 
    : IRequestHandler<UpdateWarehouseCommand,long>
{
    public async Task<long> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await context.Warehouses.FirstOrDefaultAsync(w =>w.Id == request.Id,cancellationToken )
            ?? throw new NotFoundException(nameof(Warehouse),nameof(request.Id), request.Id);

        mapper.Map(request, warehouse);
        warehouse.UpdatedAt = DateTime.UtcNow;
        await context.SaveAsync(cancellationToken);
        return warehouse.Id;
    }
}