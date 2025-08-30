namespace VoltStream.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Warehouses.Commands;
using VoltStream.Application.Features.Warehouses.Queries;
using VoltStream.WebApi.Models;

public class WarehousesController : BaseController
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateWarehouseCommand command)
       => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(long id)
        => Ok(new Response { Data = await Mediator.Send(new DeleteWarehouseCommand(id)) });

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateWarehouseCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
        => Ok(new Response { Data = await Mediator.Send(new GetAllWarehouseQuery()) });

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById(long id)
        => Ok(new Response { Data = await Mediator.Send(new GetWarehouseByIdQuery(id)) });
}
