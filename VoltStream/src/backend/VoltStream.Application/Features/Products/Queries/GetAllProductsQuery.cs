namespace VoltStream.Application.Features.Products.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Products.DTOs;

public record GetAllProductsQuery : IRequest<List<ProductDto>>;

public class GetAllProductsQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        => mapper.Map<List<ProductDto>>(await context.Products.ToListAsync(cancellationToken));
}