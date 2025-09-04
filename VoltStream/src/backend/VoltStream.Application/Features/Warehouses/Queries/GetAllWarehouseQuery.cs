namespace VoltStream.Application.Features.Warehouses.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Warehouses.DTOs;

public record GetAllWarehouseQuery : IRequest<List<WarehouseItemDTO>>;


public class GetAllWarehouseQueryHandler(
    IAppDbContext context,
    IMapper mapper) :
    IRequestHandler<GetAllWarehouseQuery, List<WarehouseItemDTO>>
{
    public async Task<List<WarehouseItemDTO>> Handle(GetAllWarehouseQuery query, CancellationToken cancellationToken)
        => mapper.Map<List<WarehouseItemDTO>>(await context.WarehouseItems.ToListAsync(cancellationToken));
}

