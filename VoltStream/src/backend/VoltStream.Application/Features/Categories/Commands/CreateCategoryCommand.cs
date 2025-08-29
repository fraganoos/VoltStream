using MediatR;
using AutoMapper;
using VoltStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;

namespace VoltStream.Application.Features.Categories.Commands;

public record CreateCategoryCommand(string name) : IRequest<long>;

public class CreateCategoryCommandHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<CreateCategoryCommand, long>
{
    public async Task<long> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await context.Products
            .AnyAsync(p => p.Name.Equals(request.name, StringComparison.CurrentCultureIgnoreCase), cancellationToken);

        if (categoryExists)
            throw new AlreadyExistException(nameof(Category));

        var category = mapper.Map<Category>(request);
        context.Categories.Add(category);
        await context.SaveAsync(cancellationToken);
        return category.Id;
    }
}