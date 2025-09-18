namespace VoltStream.Application.Features.Warehouses.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Warehouses.DTOs;

public record GetAllWarehouseQuery : IRequest<IReadOnlyCollection<WarehouseDTO>>;


public class GetAllWarehouseQueryHandler(
    IAppDbContext context,
    IMapper mapper) :
    IRequestHandler<GetAllWarehouseQuery, IReadOnlyCollection<WarehouseDTO>>
{
    public async Task<IReadOnlyCollection<WarehouseDTO>> Handle(GetAllWarehouseQuery query, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<WarehouseDTO>>(await context.Warehouses.ToListAsync(cancellationToken));
}

