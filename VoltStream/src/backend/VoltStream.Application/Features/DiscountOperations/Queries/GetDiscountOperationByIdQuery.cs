namespace VoltStream.Application.Features.DiscountOperations.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.DiscountOperations.DTOs;
using VoltStream.Domain.Entities;

public record GetDiscountOperationByIdQuery(long Id) : IRequest<DiscountOperationDto>;

public class GetDiscountOperationByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetDiscountOperationByIdQuery, DiscountOperationDto>
{
    public async Task<DiscountOperationDto> Handle(GetDiscountOperationByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<DiscountOperationDto>(await context.DiscountOperations
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken))
        ?? throw new NotFoundException(nameof(DiscountOperation), nameof(request.Id), request.Id);
}
