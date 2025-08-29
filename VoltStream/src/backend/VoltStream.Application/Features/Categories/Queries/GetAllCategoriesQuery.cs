namespace VoltStream.Application.Features.Categories.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Categories.DTOs;

public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;

public class GetAllCategoriesQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        => mapper.Map<List<CategoryDto>>(await context.Categories.ToListAsync(cancellationToken));
}