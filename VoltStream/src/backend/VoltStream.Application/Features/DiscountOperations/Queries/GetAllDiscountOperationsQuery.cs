namespace VoltStream.Application.Features.DiscountOperations.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.DiscountOperations.DTOs;

public record GetAllDiscountOperationsQuery : IRequest<List<DiscountOperationDto>>;

public class GetAllDiscountOperationsQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllDiscountOperationsQuery, List<DiscountOperationDto>>
{
    public async Task<List<DiscountOperationDto>> Handle(GetAllDiscountOperationsQuery request, CancellationToken cancellationToken)
        => mapper.Map<List<DiscountOperationDto>>(await context.DiscountsOperations.ToListAsync(cancellationToken));
}
