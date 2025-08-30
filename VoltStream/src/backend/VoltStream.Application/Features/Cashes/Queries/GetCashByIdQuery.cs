namespace VoltStream.Application.Features.Cashes.Queries;

using MediatR;
using AutoMapper;
using VoltStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Cashes.DTOs;

public record GetCashByIdQuery(long Id):IRequest<CashDTO>;


public class GetCashByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<GetCashByIdQuery, CashDTO>
{
    public async Task<CashDTO> Handle(GetCashByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<CashDTO>(await context.Cashes
                                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken))
            ?? throw new NotFoundException(nameof(Cash), nameof(request.Id), request.Id);
}