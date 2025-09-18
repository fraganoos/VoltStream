namespace VoltStream.Application.Features.WarehouseItems.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Warehouses.DTOs;
using VoltStream.Domain.Entities;

public record GetProductDetailsFromWarehouse(long id) : IRequest<List<WarehouseItemDTO>>;
internal class GetProductDetailsFromWarehouseHandler(
    IAppDbContext context, IMapper mapper
    ) : IRequestHandler<GetProductDetailsFromWarehouse, List<WarehouseItemDTO>>
{
    public async Task<List<WarehouseItemDTO>> Handle(GetProductDetailsFromWarehouse request, CancellationToken cancellationToken)
    {
        return mapper.Map<List<WarehouseItemDTO>>(await context.WarehouseItems
                 .Where(w => !w.IsDeleted && w.ProductId == request.id)
                 .ToListAsync(cancellationToken))
            ?? throw new NotFoundException(nameof(Product), nameof(request.id), request.id);
    }
}
