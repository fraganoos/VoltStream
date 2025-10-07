namespace VoltStream.Application.Features.Sales.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Sales.DTOs;
using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;

public record CreateSaleCommand(
    long CustomerId,
    DateTime OperationDate,
    decimal Discount,
    decimal Summa,
    string Description,
    List<SaleItemCreateDto> SaleItems)
    : IRequest<long>;

public class CreateSaleCommandHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<CreateSaleCommand, long>
{
    public async Task<long> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        await context.BeginTransactionAsync(cancellationToken);
        var warehouse = await context.Warehouses
            .Include(w => w.Items)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Warehouse));

        var customer = await context.Customers
            .Include(c => c.Account)
            .FirstOrDefaultAsync(a => a.Id == request.CustomerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Account));

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

            description.Append($"{product.Name} - {item.TotalQuantity} metr;");
        }

        var discountOperation = new DiscountOperation
        {
            Date = request.OperationDate,
            DiscountSumm = request.Discount,
            Description = $"Chegirma savdo uchun: {description}",
            Customer = customer
        };

        var sale = mapper.Map<Sale>(request);

        customer.Account.CurrentSumm -= sale.Summa;
        customer.Account.DiscountSumm += sale.Discount;

        var customerOperation = mapper.Map<CustomerOperation>(request);
        customerOperation.OperationType = OperationType.Sale;
        customerOperation.Customer = customer;
        customerOperation.Description = $"Savdo ID = {sale.Id}: {request.Description}. {description}".Trimmer(200);
        sale.CustomerOperation = customerOperation;
        sale.DiscountOperation = discountOperation;

        context.Sales.Add(sale);

        await context.CommitTransactionAsync(cancellationToken);

        return sale.Id;
    }
}
