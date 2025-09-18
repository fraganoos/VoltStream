namespace VoltStream.Application.Features.Supplies.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Supplies.DTOs;

public record GetAllSuppliesQuery : IRequest<IReadOnlyCollection<SupplyDTO>>;

public class GetAllSuppliesQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllSuppliesQuery, IReadOnlyCollection<SupplyDTO>>
{
    public async Task<IReadOnlyCollection<SupplyDTO>> Handle(GetAllSuppliesQuery request, CancellationToken cancellationToken)
       => mapper.Map<IReadOnlyCollection<SupplyDTO>>(await context.Supplies
           .Where(supply => supply.IsDeleted != true)
           .Include(supply => supply.Product)
                .ThenInclude(product => product.Category)
           .ToListAsync(cancellationToken));
}
