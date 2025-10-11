namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Sales.Commands;
using VoltStream.Application.Features.Sales.DTOs;
using VoltStream.Application.Features.Sales.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class SalesController
    : CrudController<SaleDto,
        GetAllSalesQuery,
        GetSaleByIdQuery,
        CreateSaleCommand,
        UpdateSaleCommand,
        DeleteSaleCommand>
{
    [HttpPost("filter")]
    public async Task<IActionResult> GetFiltered(SaleFilterQuery query)
        => Ok(new Response { Data = await Mediator.Send(query) });
}
