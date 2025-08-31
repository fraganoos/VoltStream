namespace VoltStream.Application.Features.Sales.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Sales.DTOs;

public record GetAllSalesQuery : IRequest<List<SalesDto>>;

public class GetAllSalesQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllSalesQuery, List<SalesDto>>
{
    public async Task<List<SalesDto>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        => mapper.Map<List<SalesDto>>(await context.Products.ToListAsync(cancellationToken));
}
