namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Supplies.Commands;
using VoltStream.Application.Features.Supplies.DTOs;
using VoltStream.Application.Features.Supplies.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class SuppliesController
    : CrudController<SupplyDto,
        GetAllSuppliesQuery,
        GetSupplyByIdQuery,
        CreateSupplyCommand,
        UpdateSupplyCommand,
        DeleteSupplyCommand>
{
    [HttpGet("by-date")]
    public async Task<IActionResult> GetAll([FromQuery] DateTimeOffset date)
        => Ok(new Response { Data = await Mediator.Send(new GetAllSuppliesByDateQuery(date)) });

    [HttpPost("filter")]
    public async Task<IActionResult> GetAll(SupplyFilterQuery query)
        => Ok(new Response { Data = await Mediator.Send(query) });
}
