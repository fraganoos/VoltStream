namespace VoltStream.Application.Features.Products.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record UpdateProductCommand(long Id, string Name, long CategoryId) : IRequest<bool>;

public class UpdateProductCommandHandler(
    IAppDbContext context)
    : IRequestHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), nameof(request.Id), request.Id);

        product.Name = request.Name;
        product.CategoryId = request.CategoryId;

        return await context.SaveAsync(cancellationToken) > 0;
    }
}