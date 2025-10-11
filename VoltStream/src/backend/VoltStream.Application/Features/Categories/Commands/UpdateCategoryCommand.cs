namespace VoltStream.Application.Features.Categories.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record UpdateCategoryCommand(
    long Id,
    string Name)
    : IRequest<bool>;

public class UpdateCategoryCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<UpdateCategoryCommand, bool>
{
    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), nameof(request.Id), request.Id);

        mapper.Map(request, category);
        return await context.SaveAsync(cancellationToken) > 0;
    }
}