namespace VoltStream.Application.Features.WarehouseStocks.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Warehouses.DTOs;

public record GetAllWarehouseItemQuery() : IRequest<IReadOnlyCollection<WarehouseItemDto>>;

public class GetAllWarehouseItemQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllWarehouseItemQuery, IReadOnlyCollection<WarehouseItemDto>>
{
    public async Task<IReadOnlyCollection<WarehouseItemDto>> Handle(GetAllWarehouseItemQuery request, CancellationToken cancellationToken)
         => mapper.Map<IReadOnlyCollection<WarehouseItemDto>>(await context.WarehouseStocks
             .Where(w => w.IsDeleted != true)
             .ToListAsync(cancellationToken));

}

