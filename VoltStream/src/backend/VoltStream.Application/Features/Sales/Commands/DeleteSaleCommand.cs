namespace VoltStream.Application.Features.Sales.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record DeleteSaleCommand(long Id) : IRequest<bool>;

public class DeleteSaleCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeleteSaleCommand, bool>
{
    public async Task<bool> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        await context.BeginTransactionAsync(cancellationToken);

        try
        {
            var sale = await GetSaleAsync(request.Id, cancellationToken);
            var warehouse = await GetWarehouseAsync(cancellationToken);
            var account = GetSaleAccount(sale);

            await RevertSaleStockAsync(sale, warehouse, cancellationToken);

            // Accountni revert qilish faqat IsApplied = false bo'lsa
            if (!sale.DiscountOperation.IsApplied)
            {
                account.Balance += sale.Amount;
                account.Discount -= sale.Discount;
            }

            MarkSaleAsDeleted(sale);

            return await context.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<Sale> GetSaleAsync(long saleId, CancellationToken cancellationToken)
    {
        return await context.Sales
            .Include(s => s.CustomerOperation)
            .Include(s => s.Customer)
                .ThenInclude(c => c.Accounts)
            .Include(s => s.DiscountOperation)
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == saleId, cancellationToken)
            ?? throw new NotFoundException(nameof(Sale), nameof(saleId), saleId);
    }

    private async Task<Warehouse> GetWarehouseAsync(CancellationToken cancellationToken)
    {
        return await context.Warehouses
            .Include(w => w.Stocks)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Warehouse));
    }

    private Account GetSaleAccount(Sale sale)
    {
        var account = sale.Customer.Accounts.FirstOrDefault(a => a.CurrencyId == sale.CurrencyId)
            ?? throw new ConflictException("Mijoz uchun tegishli currency account topilmadi");
        return account;
    }

    private async Task RevertSaleStockAsync(Sale sale, Warehouse warehouse, CancellationToken cancellationToken)
    {
        foreach (var item in sale.Items)
        {
            var residue = warehouse.Stocks.FirstOrDefault(r => r.ProductId == item.ProductId && r.LengthPerRoll == item.LengthPerRoll)
                ?? throw new NotFoundException(nameof(WarehouseStock), nameof(item.Id), item.Id);

            var countRoll = (int)item.TotalLength / item.LengthPerRoll;
            var residueItem = item.TotalLength % item.LengthPerRoll;

            residue.RollCount += countRoll;
            residue.TotalLength += item.TotalLength - residueItem;

            if (residueItem > 0)
                await AddOrUpdateResidueItemAsync(item, residueItem, warehouse, cancellationToken);
        }
    }

    private async Task AddOrUpdateResidueItemAsync(SaleItem item, decimal residueItem, Warehouse warehouse, CancellationToken cancellationToken)
    {
        var existItem = await context.WarehouseStocks
            .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.LengthPerRoll == residueItem, cancellationToken);

        if (existItem is null)
            context.WarehouseStocks.Add(new WarehouseStock
            {
                RollCount = 1,
                ProductId = item.ProductId,
                LengthPerRoll = residueItem,
                DiscountRate = item.DiscountRate,
                UnitPrice = item.UnitPrice,
                TotalLength = residueItem,
                Warehouse = warehouse
            });
        else
        {
            existItem.RollCount += 1;
            existItem.DiscountRate = item.DiscountRate;
            existItem.UnitPrice = item.UnitPrice;
            existItem.TotalLength += residueItem;
        }
    }

    private void MarkSaleAsDeleted(Sale sale)
    {
        sale.IsDeleted = true;
        sale.CustomerOperation.IsDeleted = true;
        sale.DiscountOperation.IsDeleted = true;
    }
}
