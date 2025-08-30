namespace VoltStream.Application.Features.Warehouses.Queries;

using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Warehouses.DTOs;

public record GetAllWarehouseQuery:IRequest<List<WarehouseDTO>>;


public class GetAllWarehouseQueryHandler(
    IAppDbContext context,
    IMapper mapper) :
    IRequestHandler<GetAllWarehouseQuery, List<WarehouseDTO>>
{
    public async Task<List<WarehouseDTO>> Handle(GetAllWarehouseQuery query, CancellationToken cancellationToken)
        => mapper.Map<List<WarehouseDTO>>(await context.Warehouses.ToListAsync(cancellationToken));
}

