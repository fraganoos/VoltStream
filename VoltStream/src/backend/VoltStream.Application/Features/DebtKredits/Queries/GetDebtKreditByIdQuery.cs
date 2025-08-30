namespace VoltStream.Application.Features.DebtKredits.Queries;

using MediatR;
using AutoMapper;
using VoltStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.DebtKredits.DTOs;

public record GetDebtKreditByIdQuery(long Id) : IRequest<DebtKreditDTO>;

public class GetDebtKreditByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<GetDebtKreditByIdQuery, DebtKreditDTO>
{
    public async Task<DebtKreditDTO> Handle(GetDebtKreditByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<DebtKreditDTO>(await context.DebtKredits
                                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken))
            ?? throw new NotFoundException(nameof(DebtKredit), nameof(request.Id), request.Id);
}