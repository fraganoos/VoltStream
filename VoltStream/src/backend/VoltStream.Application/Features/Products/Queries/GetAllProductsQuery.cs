namespace VoltStream.Application.Features.Products.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Products.DTOs;

public record GetAllProductsQuery : IRequest<List<ProductDTO>>;

public class GetAllProductsQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllProductsQuery, List<ProductDTO>>
{
    public async Task<List<ProductDTO>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        => mapper.Map<List<ProductDTO>>(await context.Products.ToListAsync(cancellationToken));
}

