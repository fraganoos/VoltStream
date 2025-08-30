namespace VoltStream.Application.Features.Warehouses.Queries;

using MediatR;
using AutoMapper;
using VoltStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Warehouses.DTOs;

public record GetWarehouseByIdQuery(long Id):IRequest<WarehouseDTO>;

public class GetWarehouseByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetWarehouseByIdQuery, WarehouseDTO>
{
    public async Task<WarehouseDTO> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<WarehouseDTO>(await context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken))
        ?? throw new NotFoundException(nameof(Warehouse), nameof(request.Id), request.Id);
}