namespace VoltStream.Application.Features.Supplies.Queries;

using AutoMapper;
using MediatR;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Models;
using VoltStream.Application.Features.Supplies.DTOs;

public record SupplyFilterQuery : FilteringRequest, IRequest<IReadOnlyCollection<SupplyDto>>;

public class SupplyFilterQueryHandler(
    IAppDbContext context,
    IPagingMetadataWriter writer,
    IMapper mapper) : IRequestHandler<SupplyFilterQuery, IReadOnlyCollection<SupplyDto>>
{
    public async Task<IReadOnlyCollection<SupplyDto>> Handle(SupplyFilterQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<SupplyDto>>(await context.Supplies
            .ToPagedListAsync(request, writer, cancellationToken));
}
