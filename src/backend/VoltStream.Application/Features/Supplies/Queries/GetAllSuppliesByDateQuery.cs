namespace VoltStream.Application.Features.Supplies.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Supplies.DTOs;

public record GetAllSuppliesByDateQuery(
    DateTimeOffset OrerationDate) : IRequest<IReadOnlyCollection<SupplyDto>>;

public class GetAllSuppliesByDateQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllSuppliesByDateQuery, IReadOnlyCollection<SupplyDto>>
{

    public async Task<IReadOnlyCollection<SupplyDto>> Handle(GetAllSuppliesByDateQuery request, CancellationToken cancellationToken)
    {
        var startLocal = request.OrerationDate.LocalDateTime.Date;   // 2025-09-15 00:00 (local)
        var endLocal = startLocal.AddDays(1);

        // PostgreSQL bilan ishlash uchun UTC ga aylantiramiz
        var startUtc = DateTime.SpecifyKind(startLocal, DateTimeKind.Local).ToUniversalTime();
        var endUtc = DateTime.SpecifyKind(endLocal, DateTimeKind.Local).ToUniversalTime();

        var supplies = mapper.Map<IReadOnlyCollection<SupplyDto>>(await context.Supplies
            .Where(s => !s.IsDeleted)
            .Where(s => s.Date >= startUtc && s.Date < endUtc)
            .Include(s => s.Product)
                .ThenInclude(p => p.Category)
            .ToListAsync(cancellationToken));
        return supplies;
    }
}
