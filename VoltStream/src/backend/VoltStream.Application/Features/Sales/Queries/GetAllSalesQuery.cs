namespace VoltStream.Application.Features.Sales.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Sales.DTOs;

public record GetAllSalesQuery : IRequest<IReadOnlyCollection<SalesDto>>;

public class GetAllSalesQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllSalesQuery, IReadOnlyCollection<SalesDto>>
{
    public async Task<IReadOnlyCollection<SalesDto>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<SalesDto>>(await context.Products.ToListAsync(cancellationToken));
}
