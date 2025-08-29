namespace VoltStream.Application.Features.Products.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Products.DTOs;
using VoltStream.Domain.Entities;

public record GetProductByIdQuery(long Id) : IRequest<ProductDTO>;

public class GetProductByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<GetProductByIdQuery, ProductDTO>
{
    public async Task<ProductDTO> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<ProductDTO>(await context.Products
                                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken))
            ?? throw new NotFoundException(nameof(Product), nameof(request.Id), request.Id);
}