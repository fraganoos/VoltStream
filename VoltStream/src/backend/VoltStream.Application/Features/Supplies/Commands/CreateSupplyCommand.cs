namespace VoltStream.Application.Features.Supplies.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record CreateSupplyCommand(
    DateTimeOffset Date,
    long CategoryId,
    string CategoryName,
    long ProductId,
    string ProductName,
    decimal RollCount,
    decimal LengthPerRoll,
    decimal TotalLength,
    decimal UnitPrice,
    decimal DiscountRate)
    : IRequest<long>;

public class CreateSupplyCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateSupplyCommand, long>
{
    public async Task<long> Handle(CreateSupplyCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await context.Warehouses
            .Include(w => w.Stocks)
            .FirstOrDefaultAsync(cancellationToken);

        if (warehouse is null)
        {
            context.Warehouses.Add(warehouse = new());
            warehouse.Stocks = [];
        }

        await context.BeginTransactionAsync(cancellationToken);

        long newCategoryId = request.CategoryId;
        if (request.CategoryId <= 0)
        {
            var category = new Category()
            {
                Name = request.CategoryName.Trim(),
                NormalizedName = request.CategoryName.ToNormalized(),
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

        var warehouseItem = warehouse.Stocks.FirstOrDefault(wh
            => wh.ProductId == newProductId && wh.LengthPerRoll == request.LengthPerRoll);


        if (warehouseItem is null)
        {
            var stock = mapper.Map<WarehouseStock>(request);
            stock.ProductId = newProductId;
            warehouse.Stocks.Add(stock);
        }
        else
        {
            warehouseItem.RollCount += request.RollCount;
            warehouseItem.TotalLength += request.TotalLength;
            warehouseItem.UnitPrice = request.UnitPrice;
            warehouseItem.DiscountRate = request.DiscountRate;
        }

        var supply = mapper.Map<Supply>(request);
        supply.ProductId = newProductId;
        context.Supplies.Add(supply);

        await context.CommitTransactionAsync(cancellationToken);
        return supply.Id;
    }
}
