namespace VoltStream.Application.Features.Supplies.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Supplies.DTOs;
using VoltStream.Domain.Entities;

public record GetSupplyByIdQuery(long Id) : IRequest<SupplyDto>;

public class GetSupplyByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetSupplyByIdQuery, SupplyDto>
{
    public async Task<SupplyDto> Handle(GetSupplyByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<SupplyDto>(await context.Supplies
            .Where(s => s.IsDeleted != true)
            .Include(supply => supply.Product)
                .ThenInclude(product => product.Category)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken))
        ?? throw new NotFoundException(nameof(Supply), nameof(request.Id), request.Id);
}
