using MediatR;
using VoltStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;

namespace VoltStream.Application.Features.Categories.Commands;

public record UpdateCategoryCommand(long Id, string Name) : IRequest<long>;

public class UpdateCategoryCommandHandler(
    IAppDbContext context)
    : IRequestHandler<UpdateCategoryCommand, long>
{
    public async Task<long> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), nameof(request.Id), request.Id);

        category.Name = request.Name;
        await context.SaveAsync(cancellationToken);
        return category.Id;
    }
}