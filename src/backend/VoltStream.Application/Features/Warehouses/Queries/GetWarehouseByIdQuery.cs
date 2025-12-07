namespace VoltStream.Application.Features.Warehouses.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Warehouses.DTOs;
using VoltStream.Domain.Entities;

public record GetWarehouseByIdQuery(long Id) : IRequest<WarehouseDto>;

public class GetWarehouseByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetWarehouseByIdQuery, WarehouseDto>
{
    public async Task<WarehouseDto> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<WarehouseDto>(await context.Warehouses
            .Include(wh => wh.Stocks)
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken))
        ?? throw new NotFoundException(nameof(Warehouse), nameof(request.Id), request.Id);
}