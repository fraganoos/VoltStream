namespace VoltStream.Application.Features.DiscountOperations.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.DiscountOperations.DTOs;

public record GetAllDiscountOperationsQuery : IRequest<IReadOnlyCollection<DiscountOperationDto>>;

public class GetAllDiscountOperationsQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllDiscountOperationsQuery, IReadOnlyCollection<DiscountOperationDto>>
{
    public async Task<IReadOnlyCollection<DiscountOperationDto>> Handle(GetAllDiscountOperationsQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<DiscountOperationDto>>(await context.DiscountOperations.ToListAsync(cancellationToken));
}
