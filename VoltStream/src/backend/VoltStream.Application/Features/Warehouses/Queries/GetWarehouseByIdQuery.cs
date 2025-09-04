namespace VoltStream.Application.Features.Warehouses.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Warehouses.DTOs;
using VoltStream.Domain.Entities;

public record GetWarehouseByIdQuery(long Id) : IRequest<WarehouseItemDTO>;

public class GetWarehouseByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetWarehouseByIdQuery, WarehouseItemDTO>
{
    public async Task<WarehouseItemDTO> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<WarehouseItemDTO>(await context.WarehouseItems
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken))
        ?? throw new NotFoundException(nameof(WarehouseItem), nameof(request.Id), request.Id);
}