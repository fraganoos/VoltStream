namespace VoltStream.Application.Features.Supplies.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record UpdateSupplyCommand(
    long Id,
    DateTimeOffset OperationDate,
    long ProductId,
    decimal CountRoll,
    decimal QuantityPerRoll,
    decimal TotalQuantity) : IRequest<long>;

public class UpdateSupplyCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<UpdateSupplyCommand, long>
{
    public async Task<long> Handle(UpdateSupplyCommand request, CancellationToken cancellationToken)
    {
        var supply = await context.Supplies.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Supply), nameof(request.Id), request.Id);

        var warehouse = await context.WarehouseItems
            .FirstOrDefaultAsync(wh => wh.ProductId == supply.ProductId &&
                wh.QuantityPerRoll == supply.QuantityPerRoll, cancellationToken)
            ?? throw new NotFoundException(nameof(WarehouseItem), nameof(request.Id), request.Id);

        if (warehouse.TotalQuantity < request.TotalQuantity - supply.TotalQuantity)
            throw new ConflictException($"Omborda bu maxsulotdan faqat {warehouse.TotalQuantity} metr mavjud.\nO'zgartirish uchun yetarli emas!");

        await context.BeginTransactionAsync(cancellationToken);

        warehouse.TotalQuantity -= supply.TotalQuantity + request.TotalQuantity;
        mapper.Map(request, supply);

        await context.CommitTransactionAsync(cancellationToken);
        return supply.Id;
    }
}