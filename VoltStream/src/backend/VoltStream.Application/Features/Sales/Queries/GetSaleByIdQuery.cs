namespace VoltStream.Application.Features.Sales.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Sales.DTOs;
using VoltStream.Domain.Entities;

public record GetSaleByIdQuery(long Id) : IRequest<SalesDto>;

public class GetSaleByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetSaleByIdQuery, SalesDto>
{
    public async Task<SalesDto> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<SalesDto>(await context.Sales
            .Include(sale => sale.SaleItems)
            .Include(s => s.CustomerOperation)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken))
        ?? throw new NotFoundException(nameof(Sale), nameof(request.Id), request.Id);
}
