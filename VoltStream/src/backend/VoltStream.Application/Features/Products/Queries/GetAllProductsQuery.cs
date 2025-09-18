namespace VoltStream.Application.Features.Products.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Products.DTOs;

public record GetAllProductsQuery : IRequest<IReadOnlyCollection<ProductDTO>>;

public class GetAllProductsQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllProductsQuery, IReadOnlyCollection<ProductDTO>>
{
    public async Task<IReadOnlyCollection<ProductDTO>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<ProductDTO>>(await context.Products.ToListAsync(cancellationToken));
}

