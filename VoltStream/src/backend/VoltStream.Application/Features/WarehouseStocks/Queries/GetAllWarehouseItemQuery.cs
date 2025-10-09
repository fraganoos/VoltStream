namespace VoltStream.Application.Features.WarehouseStocks.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.WarehouseStocks.DTOs;

public record GetAllWarehouseItemQuery() : IRequest<IReadOnlyCollection<WarehouseStockDto>>;

public class GetAllWarehouseItemQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllWarehouseItemQuery, IReadOnlyCollection<WarehouseStockDto>>
{
    public async Task<IReadOnlyCollection<WarehouseStockDto>> Handle(GetAllWarehouseItemQuery request, CancellationToken cancellationToken)
         => mapper.Map<IReadOnlyCollection<WarehouseStockDto>>(await context.WarehouseStocks
             .Where(w => w.IsDeleted != true)
             .ToListAsync(cancellationToken));

}

