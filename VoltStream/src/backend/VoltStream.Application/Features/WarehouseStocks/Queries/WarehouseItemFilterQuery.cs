namespace VoltStream.Application.Features.WarehouseStocks.Queries;

using AutoMapper;
using MediatR;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Models;
using VoltStream.Application.Features.Warehouses.DTOs;

public record WarehouseItemFilterQuery : FilteringRequest, IRequest<IReadOnlyCollection<WarehouseItemDto>>;

public class WarehouseItemFilterQueryHandler(
    IAppDbContext context,
    IPagingMetadataWriter writer,
    IMapper mapper) : IRequestHandler<WarehouseItemFilterQuery, IReadOnlyCollection<WarehouseItemDto>>
{
    public async Task<IReadOnlyCollection<WarehouseItemDto>> Handle(WarehouseItemFilterQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<WarehouseItemDto>>(await context.WarehouseStocks
            .ToPagedListAsync(request, writer, cancellationToken));
}
