namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.WarehouseStocks.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class WarehouseItemsController : BaseController
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(new Response { Data = await Mediator.Send(new GetAllWarehouseStockQuery()) });

    [HttpGet("productId={Id:long}")]
    public async Task<IActionResult> GetAllWarehouseItemByProductId(long Id)
        => Ok(new Response { Data = await Mediator.Send(new GetProductDetailsFromWarehouse(Id)) });

    [HttpPost("filter")]
    public async Task<IActionResult> GetFiltered(WarehouseStockFilterQuery query)
            => Ok(new Response { Data = await Mediator.Send(query) });
}
