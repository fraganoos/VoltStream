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
        var startLocal = request.orerationDate.LocalDateTime.Date;   // 2025-09-15 00:00 (local)
        var endLocal = startLocal.AddDays(1);

        // PostgreSQL bilan ishlash uchun UTC ga aylantiramiz
        var startUtc = DateTime.SpecifyKind(startLocal, DateTimeKind.Local).ToUniversalTime();
        var endUtc = DateTime.SpecifyKind(endLocal, DateTimeKind.Local).ToUniversalTime();

        var supplies = mapper.Map<List<SupplyDTO>>(await context.Supplies
            .Where(s => !s.IsDeleted)
            .Where(s => s.OperationDate >= startUtc && s.OperationDate < endUtc)
            .Include(s => s.Product)
                .ThenInclude(p => p.Category)
            .ToListAsync(cancellationToken));
        return supplies;
    }
}
