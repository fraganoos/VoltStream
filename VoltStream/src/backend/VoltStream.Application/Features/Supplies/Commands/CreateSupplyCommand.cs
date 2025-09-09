namespace VoltStream.Application.Features.Supplies.Commands;

using MediatR;
using AutoMapper;
using VoltStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;

public record CreateSupplyCommand(
    DateTimeOffset OperationDate,
    long CategoryId,
    string CategoryName,
    long ProductId,
    string ProductName,
    decimal CountRoll,
    decimal QuantityPerRoll,
    decimal TotalQuantity,
    decimal Price,
    decimal DiscountPercent)
    : IRequest<long>;

public class CreateSupplyCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateSupplyCommand, long>
{
    public async Task<long> Handle(CreateSupplyCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await context.Warehouses
            .Include(w => w.Items)
            .FirstOrDefaultAsync(cancellationToken);

        if (warehouse is null)
            context.Warehouses.Add(warehouse = new());

        await context.BeginTransactionAsync(cancellationToken);

        long newCategoryId = request.CategoryId;
        if (request.CategoryId <= 0)
        {
            var category = new Category() 
            {
                 Name = request.CategoryName.Trim(),
                 NormalizedName = request.CategoryName.Trim().ToUpper(),
            };
            context.Categories.Add(category);
            await context.SaveAsync(cancellationToken);
            newCategoryId = category.Id;
        }

        long newProductId = request.ProductId;
        if (request.ProductId <= 0) 
        {
            var product = new Product()
            {
                Name = request.ProductName.Trim(),
                NormalizedName = request.ProductName.Trim().ToUpper(),
                CategoryId = newCategoryId
            };
            context.Products.Add(product);
            await context.SaveAsync(cancellationToken);
            newProductId = product.Id;
        }

        var warehouseItem = warehouse!.Items.FirstOrDefault(wh
            => wh.ProductId == newProductId && wh.QuantityPerRoll == request.QuantityPerRoll);


        if (warehouseItem is null)
            warehouse.Items.Add(new WarehouseItem()
            {
                 CountRoll = request.CountRoll,
                 DiscountPercent = request.DiscountPercent,
                 Price = request.Price,
                 ProductId = newProductId,
                 QuantityPerRoll = request.QuantityPerRoll,
                 TotalQuantity = request.TotalQuantity,
            });
        else
        {
            warehouseItem.CountRoll += request.CountRoll;
            warehouseItem.TotalQuantity += request.TotalQuantity;
            warehouseItem.Price = request.Price;
            warehouseItem.DiscountPercent = request.DiscountPercent;
        }

        var supply = mapper.Map<Supply>(request);
        context.Supplies.Add(mapper.Map<Supply>(request));

        await context.CommitTransactionAsync(cancellationToken);
        return supply.Id;
    }
}