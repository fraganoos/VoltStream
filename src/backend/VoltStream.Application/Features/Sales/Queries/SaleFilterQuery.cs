namespace VoltStream.Application.Features.Sales.Queries;

using AutoMapper;
using MediatR;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Models;
using VoltStream.Application.Features.Sales.DTOs;

public record SaleFilterQuery : FilteringRequest, IRequest<IReadOnlyCollection<SaleDto>>;

public class SaleFilterQueryHandler(
    IAppDbContext context,
    IPagingMetadataWriter writer,
    IMapper mapper) : IRequestHandler<SaleFilterQuery, IReadOnlyCollection<SaleDto>>
{
    public async Task<IReadOnlyCollection<SaleDto>> Handle(SaleFilterQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<SaleDto>>(await context.Sales
            .ToPagedListAsync(request, writer, cancellationToken));
}
