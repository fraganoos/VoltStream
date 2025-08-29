namespace VoltStream.Application.Features.Products.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record DeleteProductCommand(long Id) : IRequest<bool>;

public class DeleteProductCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), nameof(request.Id), request.Id);

        product.IsDeleted = true;
        context.Products.Update(product);
        return await context.SaveAsync(cancellationToken) > 0;
    }
}
