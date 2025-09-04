namespace VoltStream.Application.Features.CustomerOperations.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.CustomerOperations.DTOs;

public record GetAllCustomerOperationsQuery : IRequest<List<CustomerOperationDto>>;

public class GetAllCustomerOperationsQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllCustomerOperationsQuery, List<CustomerOperationDto>>
{
    public async Task<List<CustomerOperationDto>> Handle(GetAllCustomerOperationsQuery request, CancellationToken cancellationToken)
        => mapper.Map<List<CustomerOperationDto>>(await context.CustomerOperations
            .ToListAsync(cancellationToken));
}
