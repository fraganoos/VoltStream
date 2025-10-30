﻿namespace VoltStream.Application.Features.Products.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record GetAllProductsByCategoryIdQuery(long CategoryId) : IRequest<IReadOnlyCollection<Product>>;

public class GetAllProductsByCategoryIdQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllProductsByCategoryIdQuery, IReadOnlyCollection<Product>>
{
    public async Task<IReadOnlyCollection<Product>> Handle(GetAllProductsByCategoryIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<Product>>
        (await context.Products.Where(a => a.CategoryId == request.CategoryId).ToListAsync(cancellationToken));
}
