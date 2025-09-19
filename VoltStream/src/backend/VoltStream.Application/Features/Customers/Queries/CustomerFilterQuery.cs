namespace VoltStream.Application.Features.Customers.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Models;
using VoltStream.Application.Features.Customers.DTOs;

public record CustomerFilterQuery : FilteringRequest, IRequest<IReadOnlyCollection<CustomerDto>>;

public class CustomerFilterQueryHandler(
    IAppDbContext context,
    IPagingMetadataWriter writer,
    IMapper mapper) : IRequestHandler<CustomerFilterQuery, IReadOnlyCollection<CustomerDto>>
{
    public async Task<IReadOnlyCollection<CustomerDto>> Handle(CustomerFilterQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<CustomerDto>>(await context.Customers
            .Include(c => c.Account)
            .ToPagedListAsync(request, writer, cancellationToken));
}
