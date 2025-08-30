namespace VoltStream.Application.Features.Supplies.Mappers;

using MediatR;
using AutoMapper;
using VoltStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Supplies.DTOs;

public record GetSupplyByIdQuery(long Id) : IRequest<SupplyDTO>;

public class GetSupplyByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<GetSupplyByIdQuery, SupplyDTO>
{
    public async Task<SupplyDTO> Handle(GetSupplyByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<SupplyDTO>(await context.Supplies
                                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
        ?? throw new NotFoundException(nameof(Supply), nameof(request.Id), request.Id));
}
