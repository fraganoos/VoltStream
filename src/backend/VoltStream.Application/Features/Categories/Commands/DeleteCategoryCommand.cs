namespace VoltStream.Application.Features.Categories.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record DeleteCategoryCommand(long Id) : IRequest<bool>;

public class DeleteCategoryCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeleteCategoryCommand, bool>
{
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), nameof(request.Id), request.Id);

        context.Categories.Remove(category);

        try
        {
            await context.SaveAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenException("Ushbu kategoriyada mahsulotlar mavjud yoki tizim xatoligi tufayli o'chirib bo'lmaydi.");
        }
        
        return true;
    }
}
