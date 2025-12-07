namespace VoltStream.Application.Features.WarehouseStocks.Queries;

using AutoMapper;
using MediatR;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Models;
using VoltStream.Application.Features.WarehouseStocks.DTOs;

public record WarehouseStockFilterQuery : FilteringRequest, IRequest<IReadOnlyCollection<WarehouseStockDto>>;

public class WarehouseStockFilterQueryHandler(
    IAppDbContext context,
    IPagingMetadataWriter writer,
    IMapper mapper) : IRequestHandler<WarehouseStockFilterQuery, IReadOnlyCollection<WarehouseStockDto>>
{
    public async Task<IReadOnlyCollection<WarehouseStockDto>> Handle(WarehouseStockFilterQuery request, CancellationToken cancellationToken)
    {
        var d = mapper.Map<IReadOnlyCollection<WarehouseStockDto>>(await context.WarehouseStocks
            .ToPagedListAsync(request, writer, cancellationToken));

        return d;
    }

}
