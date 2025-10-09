namespace VoltStream.Application.Features.WarehouseStocks.Queries;

using AutoMapper;
using MediatR;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Models;
using VoltStream.Application.Features.WarehouseStocks.DTOs;

public record WarehouseItemFilterQuery : FilteringRequest, IRequest<IReadOnlyCollection<WarehouseStockDto>>;

public class WarehouseItemFilterQueryHandler(
    IAppDbContext context,
    IPagingMetadataWriter writer,
    IMapper mapper) : IRequestHandler<WarehouseItemFilterQuery, IReadOnlyCollection<WarehouseStockDto>>
{
    public async Task<IReadOnlyCollection<WarehouseStockDto>> Handle(WarehouseItemFilterQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<WarehouseStockDto>>(await context.WarehouseStocks
            .ToPagedListAsync(request, writer, cancellationToken));
}
