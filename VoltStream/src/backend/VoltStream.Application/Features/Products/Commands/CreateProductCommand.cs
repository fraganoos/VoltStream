namespace VoltStream.Application.Features.Products.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record CreateProductCommand(string Name, long CategoryId) : IRequest<long>;

public class CreateProductCommandHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<CreateProductCommand, long>
{
    public async Task<long> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var productExists = await context.Products
            .AnyAsync(p => p.Name.Equals(request.Name, StringComparison.CurrentCultureIgnoreCase), cancellationToken);

        if (productExists)
            throw new AlreadyExistException(nameof(Product));

        var product = mapper.Map<Product>(request);
        context.Products.Add(product);
        return await context.SaveAsync(cancellationToken).ContinueWith(product => product.Id);
    }
}