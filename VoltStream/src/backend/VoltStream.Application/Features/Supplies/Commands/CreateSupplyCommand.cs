namespace VoltStream.Application.Features.Supplies.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record CreateSupplyCommand(
    DateTime Date,
    string CategoryName,
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
        await context.BeginTransactionAsync(cancellationToken);

        try
        {
            // Ombor olish yoki yaratish
            var warehouse = await context.Warehouses
                .Include(w => w.Stocks)
                .FirstOrDefaultAsync(cancellationToken);

            if (warehouse is null)
            {
                warehouse = new Warehouse();
                context.Warehouses.Add(warehouse);
                await context.SaveAsync(cancellationToken);
            }

            // Category tekshirish/yaratish
            var category = await context.Categories
                .FirstOrDefaultAsync(c => c.NormalizedName == request.CategoryName.Trim().ToUpper(), cancellationToken);

            if (category is null)
            {
                category = new Category
                {
                    Name = request.CategoryName.Trim(),
                    NormalizedName = request.CategoryName.Trim().ToUpper()
                };
                context.Categories.Add(category);
                await context.SaveAsync(cancellationToken);
            }

            // Product tekshirish/yaratish
            var product = await context.Products
                .FirstOrDefaultAsync(p => p.NormalizedName == request.ProductName.Trim().ToUpper(), cancellationToken);

            if (product is null)
            {
                product = new Product
                {
                    Name = request.ProductName.Trim(),
                    NormalizedName = request.ProductName.Trim().ToUpper(),
                    CategoryId = category.Id
                };
                context.Products.Add(product);
                await context.SaveAsync(cancellationToken);
            }

            // WarehouseStock tekshirish/yangilash
            var stock = warehouse.Stocks.FirstOrDefault(ws =>
                ws.ProductId == product.Id && ws.LengthPerRoll == request.LengthPerRoll);

            if (stock is null)
            {
                warehouse.Stocks.Add(new WarehouseStock
                {
                    ProductId = product.Id,
                    Warehouse = warehouse,
                    RollCount = request.RollCount,
                    LengthPerRoll = request.LengthPerRoll,
                    TotalLength = request.TotalLength,
                    UnitPrice = request.UnitPrice,
                    DiscountRate = request.DiscountRate
                });
            }
            else
            {
                stock.RollCount += request.RollCount;
                stock.TotalLength += request.TotalLength;
                stock.UnitPrice = request.UnitPrice;
                stock.DiscountRate = request.DiscountRate;
            }

            // Supply yaratish
            var supply = mapper.Map<Supply>(request);
            supply.ProductId = product.Id;
            context.Supplies.Add(supply);

            await context.CommitTransactionAsync(cancellationToken);
            return supply.Id;
        }
        catch
        {
            await context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
