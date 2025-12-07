namespace VoltStream.Application.Features.Customers.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Accounts.DTOs;
using VoltStream.Domain.Entities;

public record CreateCustomerCommand(
    string Name,
    string? Phone,
    string? Address,
    string? Description,
    List<AccountCommandDto> Accounts)
    : IRequest<long>;

public class CreateCustomerComandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateCustomerCommand, long>
{
    public async Task<long> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var isExist = await context.Customers
                    .AnyAsync(user => user.NormalizedName == request.Name.ToNormalized(), cancellationToken);

        if (isExist)
            throw new AlreadyExistException(nameof(Customer), nameof(request.Name), request.Name);

        var currency = await context.Currencies.FirstOrDefaultAsync(c => c.IsDefault, cancellationToken)
            ?? throw new NotFoundException(nameof(Currency), nameof(Currency.IsDefault), true);

        var customer = mapper.Map<Customer>(request);
        customer.Accounts.First().Currency = currency;
        context.Customers.Add(customer);

        await context.SaveAsync(cancellationToken);
        return customer.Id;
    }
}