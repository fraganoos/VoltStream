namespace VoltStream.Application.Features.Customers.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Customers.DTOs;

public record GetAllCustomersQuery : IRequest<List<CustomerDto>>;

public class GetAllCustomersQueryHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<GetAllCustomersQuery, List<CustomerDto>>
{
    public async Task<List<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        => mapper.Map<List<CustomerDto>>(await context.Customers.ToListAsync(cancellationToken));
}
