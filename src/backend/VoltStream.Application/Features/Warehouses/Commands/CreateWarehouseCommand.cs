namespace VoltStream.Application.Features.Warehouses.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record CreateWarehouseCommand(
    string Name)
    : IRequest<long>;

public class CreateWarehouseCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateWarehouseCommand, long>
{
    public async Task<long> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        _ = await context.Warehouses.FirstOrDefaultAsync(wh => wh.NormalizedName == request.Name, cancellationToken)
            ?? throw new AlreadyExistException(nameof(Warehouse), nameof(request.Name), request.Name);

        var warehouse = mapper.Map<Warehouse>(request);
        context.Warehouses.Add(warehouse);
        await context.SaveAsync(cancellationToken);
        return warehouse.Id;
    }
}
