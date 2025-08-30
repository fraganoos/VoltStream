namespace VoltStream.Application.Features.DebtKredits.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.DebtKredits.DTOs;

public record GetAllDebtKreditQuery : IRequest<List<DebtKreditDTO>>;

public class GetAllDebtKreditQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllDebtKreditQuery, List<DebtKreditDTO>>
{
    public async Task<List<DebtKreditDTO>> Handle(GetAllDebtKreditQuery request, CancellationToken cancellationToken)
        => mapper.Map<List<DebtKreditDTO>>(await context.DebtKredits.ToListAsync(cancellationToken));
}
