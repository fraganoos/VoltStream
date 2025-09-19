namespace VoltStream.Application.Features.Customers.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record CreateCustomerCommand(
    string Name,
    string? Phone,
    string? Address,
    string? Description,
    decimal BeginningSum,
    decimal DiscountSumm)
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

        var customer = mapper.Map<Customer>(request);
        context.Accounts.Add(new()
        {
            BeginningSumm = request.BeginningSum,
            Customer = customer,
            CurrentSumm = request.BeginningSum,
            DiscountSumm = request.DiscountSumm
        });
        context.Customers.Add(customer);

        await context.SaveAsync(cancellationToken);
        return customer.Id;
    }
}