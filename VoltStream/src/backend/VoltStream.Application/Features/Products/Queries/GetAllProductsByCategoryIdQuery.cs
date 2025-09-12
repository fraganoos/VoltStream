namespace VoltStream.Application.Features.Products.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record GetAllProductsByCategoryIdQuery(long categoryId) : IRequest<List<Product>>;

public class GetAllProductsByCategoryIdQueryHandler(
    IAppDbContext context,
    IMapper mapper) 
    : IRequestHandler<GetAllProductsByCategoryIdQuery, List<Product>>
{
    public async Task<List<Product>> Handle(GetAllProductsByCategoryIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<List<Product>>
        (await context.Products.Where(a => 
        a.CategoryId == request.categoryId).ToListAsync(cancellationToken));
}
