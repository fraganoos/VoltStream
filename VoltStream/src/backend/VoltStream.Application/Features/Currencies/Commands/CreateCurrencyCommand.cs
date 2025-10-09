namespace VoltStream.Application.Features.Currencies.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record CreateCurrencyCommand(
    string Name,
    string Code,
    string Symbol,
    decimal ExchangeRate)
    : IRequest<long>;

public class CreateCurrencyCommandHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<CreateCurrencyCommand, long>
{
    public async Task<long> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var CurrencyExists = await context.Categories
            .AnyAsync(p => p.NormalizedName == request.Name.ToNormalized(), cancellationToken);

        if (CurrencyExists)
            throw new AlreadyExistException(nameof(Currency));

        var currency = mapper.Map<Currency>(request);
        context.Currencies.Add(currency);
        await context.SaveAsync(cancellationToken);
        return currency.Id;
    }
}