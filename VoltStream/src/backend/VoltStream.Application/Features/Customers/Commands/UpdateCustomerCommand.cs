namespace VoltStream.Application.Features.Customers.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record UpdateCustomerCommand(
    long Id,
    string Name,
    string? Phone,
    string? Address,
    string? Description) : IRequest<long>;

public class UpdateCustomerCommandHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<UpdateCustomerCommand, long>
{
    public async Task<long> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), nameof(request.Id), request.Id);

        mapper.Map(request, customer);
        await context.SaveAsync(cancellationToken);

        return customer.Id;
    }
}
