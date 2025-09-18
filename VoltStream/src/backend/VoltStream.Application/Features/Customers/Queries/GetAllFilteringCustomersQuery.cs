namespace VoltStream.Application.Features.Customers.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Models;
using VoltStream.Application.Features.Customers.DTOs;

public record GetAllFilteringCustomersQuery : FilteringRequest, IRequest<IReadOnlyCollection<CustomerDto>>;

public class GetAllFilteringCustomersQueryHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<GetAllFilteringCustomersQuery, IReadOnlyCollection<CustomerDto>>
{
    public async Task<IReadOnlyCollection<CustomerDto>> Handle(GetAllFilteringCustomersQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<CustomerDto>>(await context.Customers
            .AsQueryable()
            .AsFilterable(request)
            .ToListAsync(cancellationToken));
}
