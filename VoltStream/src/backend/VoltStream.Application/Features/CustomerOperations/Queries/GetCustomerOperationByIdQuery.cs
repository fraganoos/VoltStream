namespace VoltStream.Application.Features.CustomerOperations.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.CustomerOperations.DTOs;
using VoltStream.Domain.Entities;

public record GetCustomerOperationByIdQuery(long Id) : IRequest<CustomerOperationDto>;

public class GetCustomerOperationByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetCustomerOperationByIdQuery, CustomerOperationDto>
{
    public async Task<CustomerOperationDto> Handle(GetCustomerOperationByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<CustomerOperationDto>(await context.CustomerOperations
            .Include(co => co.Account)
            .FirstOrDefaultAsync(co => co.Id == request.Id, cancellationToken))
        ?? throw new NotFoundException(nameof(CustomerOperation), nameof(request.Id), request.Id);
}