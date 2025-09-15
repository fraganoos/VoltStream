namespace VoltStream.Application.Features.WarehouseItems.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Warehouses.DTOs;

public record GetAllWarehouseItemQuery() : IRequest<List<WarehouseItemDTO>>;

public class GetAllWarehouseItemQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllWarehouseItemQuery, List<WarehouseItemDTO>>
{
    public async Task<List<WarehouseItemDTO>> Handle(GetAllWarehouseItemQuery request, CancellationToken cancellationToken)
         => mapper.Map<List<WarehouseItemDTO>>(await context.WarehouseItems
             .Where(w => w.IsDeleted != true)
             .ToListAsync(cancellationToken));

}

