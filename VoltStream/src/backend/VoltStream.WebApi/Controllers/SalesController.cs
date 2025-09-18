namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Sales.Commands;
using VoltStream.Application.Features.Sales.DTOs;
using VoltStream.Application.Features.Sales.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class SalesController
    : CrudController<SalesDto,
        GetAllSalesQuery,
        GetSaleByIdQuery,
        CreateSaleCommand,
        UpdateSaleCommand,
        DeleteSaleCommand>
{
    [HttpGet("{Id:long}")]
    public async Task<IActionResult> Get(long Id)
        => Ok(new Response { Data = await Mediator.Send(new GetSaleByIdQuery(Id)) });
}
