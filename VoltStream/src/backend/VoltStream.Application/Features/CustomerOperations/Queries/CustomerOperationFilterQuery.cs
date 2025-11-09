namespace VoltStream.Application.Features.CustomerOperations.Queries;

using AutoMapper;
using MediatR;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Models;
using VoltStream.Application.Features.CustomerOperations.DTOs;

public record CustomerOperationFilterQuery : FilteringRequest, IRequest<IReadOnlyCollection<CustomerOperationDto>>;

public class CustomerOperationFilterQueryHandler(
    IAppDbContext context,
    IPagingMetadataWriter writer,
    IMapper mapper) : IRequestHandler<CustomerOperationFilterQuery, IReadOnlyCollection<CustomerOperationDto>>
{
    public async Task<IReadOnlyCollection<CustomerOperationDto>> Handle(CustomerOperationFilterQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<CustomerOperationDto>>(await context.CustomerOperations
            .ToPagedListAsync(request, writer, cancellationToken));
}
