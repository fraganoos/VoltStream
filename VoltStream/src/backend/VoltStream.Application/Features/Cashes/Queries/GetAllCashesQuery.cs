namespace VoltStream.Application.Features.Cashes.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Cashes.DTOs;

public record GetAllCashesQuery : IRequest<List<CashDTO>>;


public class GetAllCashesQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllCashesQuery, List<CashDTO>>
{
    public async Task<List<CashDTO>> Handle(GetAllCashesQuery request, CancellationToken cancellationToken)
        => mapper.Map<List<CashDTO>>(await context.Cashes.ToListAsync(cancellationToken));
}