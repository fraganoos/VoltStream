namespace VoltStream.Application.Features.WarehouseStocks.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.WarehouseStocks.DTOs;
using VoltStream.Domain.Entities;

public record GetProductDetailsFromWarehouse(long id) : IRequest<List<WarehouseStockDto>>;
internal class GetProductDetailsFromWarehouseHandler(
    IAppDbContext context, IMapper mapper
    ) : IRequestHandler<GetProductDetailsFromWarehouse, List<WarehouseStockDto>>
{
    public async Task<List<WarehouseStockDto>> Handle(GetProductDetailsFromWarehouse request, CancellationToken cancellationToken)
    {
        return mapper.Map<List<WarehouseStockDto>>(await context.WarehouseStocks
                 .Where(w => !w.IsDeleted && w.ProductId == request.id)
                 .ToListAsync(cancellationToken))
            ?? throw new NotFoundException(nameof(Product), nameof(request.id), request.id);
    }
}
