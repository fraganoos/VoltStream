namespace VoltStream.Application.Features.Products.Queries;

using AutoMapper;
using MediatR;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Models;
using VoltStream.Application.Features.Products.DTOs;

public record ProductFilterQuery : FilteringRequest, IRequest<IReadOnlyCollection<ProductDto>>;

public class ProductFilterQueryHandler(
    IAppDbContext context,
    IPagingMetadataWriter writer,
    IMapper mapper) : IRequestHandler<ProductFilterQuery, IReadOnlyCollection<ProductDto>>
{
    public async Task<IReadOnlyCollection<ProductDto>> Handle(ProductFilterQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<ProductDto>>(await context.Products
            .ToPagedListAsync(request, writer, cancellationToken));
}
