namespace VoltStream.WebApi.Controllers;

using VoltStream.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Supplies.Mappers;
using VoltStream.Application.Features.Supplies.Queries;
using VoltStream.Application.Features.Supplies.Commands;

public class SuppliesController : BaseController
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateSupplyCommand command)
    => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateSupplyCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpDelete("delete/{Id:long}")]
    public async Task<IActionResult> Delete(long Id)
        => Ok(new Response { Data = await Mediator.Send(new DeleteSupplyCommand(Id)) });

    [HttpGet("{Id:long}")]
    public async Task<IActionResult> GetById(long Id)
        => Ok(new Response { Data = await Mediator.Send(new GetSupplyByIdQuery(Id)) });

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(new Response { Data = await Mediator.Send(new GetAllSuppliesQuery()) });
}
