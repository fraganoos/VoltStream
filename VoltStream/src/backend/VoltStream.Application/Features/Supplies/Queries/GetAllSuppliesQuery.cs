namespace VoltStream.Application.Features.Supplies.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Supplies.DTOs;

public record GetAllSuppliesQuery : IRequest<List<SupplyDTO>>;

public class GetAllSuppliesQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllSuppliesQuery, List<SupplyDTO>>
{
    public async Task<List<SupplyDTO>> Handle(GetAllSuppliesQuery request, CancellationToken cancellationToken)
       => mapper.Map<List<SupplyDTO>>(await context.Supplies
           .Where(supply=>supply.IsDeleted != true)
           .Include(supply => supply.Product)
                .ThenInclude(product=>product.Category)
           .ToListAsync(cancellationToken));
}
