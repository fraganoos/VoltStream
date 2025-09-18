namespace VoltStream.Application.Features.Cashes.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Cashes.DTOs;

public record GetAllCashesQuery : IRequest<IReadOnlyCollection<CashDTO>>;


public class GetAllCashesQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllCashesQuery, IReadOnlyCollection<CashDTO>>
{
    public async Task<IReadOnlyCollection<CashDTO>> Handle(GetAllCashesQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<CashDTO>>(await context.Cashes.ToListAsync(cancellationToken));
}