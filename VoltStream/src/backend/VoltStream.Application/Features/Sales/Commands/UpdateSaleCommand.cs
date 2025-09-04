namespace VoltStream.Application.Features.Sales.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;

public record UpdateSaleCommand(
    long Id,
    DateTimeOffset OperationDate,
    long CustomerId,
    decimal TotalQuantity,
    decimal Summa,
    long CustomerOperationId,
    decimal? Discount,
    string Description,
    List<SaleItem> SaleItems)
    : IRequest<bool>;

public class UpdateSaleCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<UpdateSaleCommand, bool>
{
    public async Task<bool> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {

        var sale = await context.Sales
            .Include(s => s.CustomerOperation)
            .Include(s => s.Customer)
                .ThenInclude(c => c.Account)
            .Include(s => s.Discount)
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Sale), nameof(request.Id), request.Id);

        var warehouse = await context.Warehouses
            .Include(wh => wh.Items)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Warehouse));

        #region Eski savdoni joyiga qaytarish

        foreach (var item in sale.SaleItems)
        {
            var residue = warehouse.Items.FirstOrDefault(r => r.ProductId == item.ProductId && r.QuantityPerRoll == item.QuantityPerRoll)
                ?? throw new NotFoundException(nameof(WarehouseItem), nameof(item.Id), item.Id);

            var countRoll = (int)item.TotalQuantity / item.QuantityPerRoll;
            var residueItem = item.TotalQuantity % item.QuantityPerRoll;

            residue.CountRoll += countRoll;
            residue.TotalQuantity += item.TotalQuantity - residueItem;

            if (residueItem > 0)
            {
                var existItem = await context.WarehouseItems.FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.QuantityPerRoll == residueItem, cancellationToken);

                if (existItem is null)
                    context.WarehouseItems.Add(new()
                    {
                        CountRoll = 1,
                        ProductId = item.ProductId,
                        QuantityPerRoll = residueItem,
                        DiscountPercent = item.DiscountPersent,
                        Price = item.Price,
                        TotalQuantity = residueItem,
                        Warehouse = warehouse
                    });
                else
                {
                    existItem.CountRoll += 1;
                    existItem.DiscountPercent = item.DiscountPersent;
                    existItem.Price = item.Price;
                    existItem.TotalQuantity += residueItem;
                }
            }
        }

        sale.Customer.Account.CurrentSumm += sale.Summa;
        sale.Customer.Account.DiscountSumm -= sale.Discount;

        #endregion

        #region Yangilangan savdoni shakllantirish

        var description = new StringBuilder();
        foreach (var item in request.SaleItems)
        {
            var residue = warehouse.Items.FirstOrDefault(r => r.ProductId == item.ProductId && r.QuantityPerRoll == item.QuantityPerRoll)
                ?? throw new NotFoundException(nameof(WarehouseItem), nameof(item.Id), item.Id);

            if (residue.TotalQuantity < item.TotalQuantity)
                throw new ConflictException($"Omborda faqat {residue.TotalQuantity} miqdorda mahsulot bor");

            residue.CountRoll -= item.CountRoll;
            residue.TotalQuantity -= item.TotalQuantity;

            if (item.QuantityPerRoll * item.CountRoll != item.TotalQuantity)
            {
                var detail = item.QuantityPerRoll * item.CountRoll - item.TotalQuantity;
                var existItem = await context.WarehouseItems.FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.QuantityPerRoll == detail, cancellationToken);

                if (existItem is null)
                    context.WarehouseItems.Add(new()
                    {
                        CountRoll = 1,
                        ProductId = item.ProductId,
                        QuantityPerRoll = detail,
                        DiscountPercent = item.DiscountPersent,
                        Price = item.Price,
                        TotalQuantity = detail,
                        Warehouse = warehouse
                    });
                else
                {
                    existItem.CountRoll += 1;
                    existItem.DiscountPercent = item.DiscountPersent;
                    existItem.Price = item.Price;
                    existItem.TotalQuantity += detail;
                }
            }

            var product = context.Products.FirstOrDefault(p => p.Id == item.ProductId)
                ?? throw new NotFoundException(nameof(Product), nameof(item.Id), item.ProductId);

            description.Append($"{product.Name} - {item.TotalQuantity} metr; ");
        }

        await context.BeginTransactionAsync(cancellationToken);

        var account = sale.Customer.Account;
        account.CurrentSumm -= sale.Summa;
        account.DiscountSumm += sale.Discount;

        mapper.Map(request, sale);
        var customerOperation = mapper.Map<CustomerOperation>(request);
        customerOperation.OperationType = OperationType.Sale;
        customerOperation.Account = account;
        customerOperation.Description = $"Savdo ID = {sale.Id}: {request.Description}. {description}".Trimmer(200);
        sale.CustomerOperation = customerOperation;

        return await context.CommitTransactionAsync(cancellationToken);

        #endregion
    }
}
