namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.WarehouseItems.Queries;
using VoltStream.WebApi.Models;

public class WarehouseItemsController : BaseController
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(new Response { Data = await Mediator.Send(new GetAllWarehouseItemQuery()) });
    [HttpGet("productId={Id:long}")]
    public async Task<IActionResult> GetAllWarehouseItemByProductId(long Id)
        => Ok(new Response { Data = await Mediator.Send(new GetProductDetailsFromWarehouse(Id)) });
}
