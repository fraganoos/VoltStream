namespace VoltStream.Application.Features.Customers.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record GetCustomerByIdQuery(long Id) : IRequest<Customer>;

public class GetCustomerByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetCustomerByIdQuery, Customer>
{
    public async Task<Customer> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<Customer>(await context.Customers
            .Include(c => c.Account)
                .ThenInclude(a => a.CustomerOperations)
            .Include(c => c.Account)
                .ThenInclude(a => a.DiscountOperations)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken))
        ?? throw new NotFoundException(nameof(Customer), nameof(request.Id), request.Id);
}
