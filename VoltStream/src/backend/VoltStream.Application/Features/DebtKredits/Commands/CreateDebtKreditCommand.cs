namespace VoltStream.Application.Features.DebtKredits.Commands;

using AutoMapper;
using MediatR;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record CreateDebtKreditCommand(
    long CustomerId,
    decimal BeginSumm,
    decimal CurrencySumm,
    bool IsActive) : IRequest<long>;

public class CreateDebtKreditCommandHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<CreateDebtKreditCommand, long>
{
    public async Task<long> Handle(CreateDebtKreditCommand request, CancellationToken cancellationToken)
    {
        var debtKredit = mapper.Map<DebtKredit>(request);
        context.DebtKredits.Add(debtKredit);
        return await context.SaveAsync(cancellationToken).ContinueWith(debtKredit => debtKredit.Id);
    }
}