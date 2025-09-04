namespace VoltStream.Application.Features.Warehouses.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record UpdateWarehouseCommand(long Id, string Name) : IRequest<long>;

public class UpdateWarehouseCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<UpdateWarehouseCommand, long>
{
    public async Task<long> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await context.Warehouses.FirstOrDefaultAsync(wh => wh.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Warehouse), nameof(request.Id), request.Id);

        mapper.Map(request, warehouse);
        await context.SaveAsync(cancellationToken);
        return warehouse.Id;
    }
}
