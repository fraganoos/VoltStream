namespace VoltStream.Application.Features.Supplies.Queries;

using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Supplies.DTOs;

public record GetAllSuppliesByDateQuery(
    DateTimeOffset orerationDate) : IRequest<List<SupplyDTO>>;

public class GetAllSuppliesByDateQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllSuppliesByDateQuery, List<SupplyDTO>>
{

    public async Task<List<SupplyDTO>> Handle(GetAllSuppliesByDateQuery request, CancellationToken cancellationToken)
    {

        var date = request.orerationDate.Date;
        var nextDay = date.AddDays(1);

        var supplies = mapper.Map<List<SupplyDTO>>(await context.Supplies
                            .Where(supply => supply.IsDeleted != true)
                            .Where(d => d.OperationDate >= date && d.OperationDate < nextDay)
                            .Include(supply => supply.Product)
                                .ThenInclude(product => product.Category)
                            .ToListAsync(cancellationToken));
        return supplies;
    }
}
